#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

SERVERS=${SERVERS:-"spring"}
CPUS_LIST=${CPUS_LIST:-"1 2"}
RUNS=${RUNS:-1}
COOLDOWN_S=${COOLDOWN_S:-60}
export DURATION_S=${DURATION_S:-120}
export WARMUP_S=${WARMUP_S:-30}
export VUS=${VUS:-50}

declare -A PORTS=([spring]=8081 [python]=8082 [csharp]=8083)

RESULTS_DIR="results/$(date +%Y%m%d-%H%M%S)"
mkdir -p "$RESULTS_DIR"
CSV="$RESULTS_DIR/summary.csv"
echo "server,cpus,run,requests,rps,avg_ms,p90_ms,p95_ms,p99_ms,max_ms,failed_rate" > "$CSV"

stop_all() {
  docker compose --profile "*" down -v >/dev/null 2>&1 || true
}
trap stop_all EXIT

wait_for_health() {
  local url=$1
  for _ in $(seq 1 60); do
    if curl -sf "$url/health" >/dev/null; then
      return 0
    fi
    sleep 2
  done
  return 1
}

K6_CMD=(k6 run k6/load.js)
if command -v taskset >/dev/null 2>&1 && [ "$(nproc)" -ge 4 ]; then
  K6_CMD=(taskset -c 3 k6 run k6/load.js)
fi

first_run=1
for server in $SERVERS; do
  port=${PORTS[$server]}
  for cpus in $CPUS_LIST; do
    for run in $(seq 1 "$RUNS"); do
      if [ "$first_run" -eq 0 ] && [ "$COOLDOWN_S" -gt 0 ]; then
        sleep "$COOLDOWN_S"
      fi
      first_run=0

      echo
      echo "=== $server | $cpus CPU | run $run ==="

      stop_all
      docker compose up -d --wait postgres
      APP_CPUS=$cpus docker compose --profile "$server" up -d --build "${server}-server"

      if ! wait_for_health "http://localhost:$port"; then
        echo "Server $server did not become healthy"
        docker compose logs "${server}-server" | tail -30
        exit 1
      fi

      out="$RESULTS_DIR/${server}_${cpus}cpu_run${run}.json"
      BASE_URL="http://localhost:$port" OUT="$out" "${K6_CMD[@]}"

      jq -r --arg s "$server" --arg c "$cpus" --arg r "$run" \
        '[$s,$c,$r,.requests,.rps,.avg,.p90,.p95,.p99,.max,.failed] | @csv' \
        "$out" | tr -d '"' >> "$CSV"

      stop_all
    done
  done
done

echo
echo "=== SUMMARY ($CSV) ==="
column -s, -t < "$CSV"