import http from 'k6/http';
import { check } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:8081';
const VUS = Number(__ENV.VUS || 50);
const DURATION_S = Number(__ENV.DURATION_S || 120);
const WARMUP_S = Number(__ENV.WARMUP_S || 30);
const OUT = __ENV.OUT || 'results/summary.json';
const MAX_ID = 10000;
const JSON_HEADERS = { headers: { 'Content-Type': 'application/json' } };

http.setResponseCallback(http.expectedStatuses({ min: 200, max: 299 }, 409));

export const options = {
  summaryTrendStats: ['avg', 'med', 'p(90)', 'p(95)', 'p(99)', 'max'],
  scenarios: {
    warmup: {
      executor: 'constant-vus',
      exec: 'shop',
      vus: 10,
      duration: `${WARMUP_S}s`,
      tags: { phase: 'warmup' },
    },
    load: {
      executor: 'constant-vus',
      exec: 'shop',
      vus: VUS,
      duration: `${DURATION_S}s`,
      startTime: `${WARMUP_S}s`,
      tags: { phase: 'load' },
    },
  },
  thresholds: {
    'http_req_duration{phase:load}': ['max>=0'],
    'http_req_failed{phase:load}': ['rate>=0'],
    'http_reqs{phase:load}': ['count>=0'],
  },
};

function rnd(n) {
  return 1 + Math.floor(Math.random() * n);
}

function listProducts() {
  const res = http.get(`${BASE_URL}/products?page=${rnd(100) - 1}&size=20`);
  check(res, { 'list 200': (r) => r.status === 200 });
}

function getProduct() {
  const res = http.get(`${BASE_URL}/products/${rnd(MAX_ID)}`);
  check(res, { 'product 200': (r) => r.status === 200 });
}

function searchProducts() {
  const res = http.get(`${BASE_URL}/products/search?q=Product%20${rnd(MAX_ID)}&size=20`);
  check(res, { 'search 200': (r) => r.status === 200 });
}

function categoryStats() {
  const res = http.get(`${BASE_URL}/stats/categories`);
  check(res, { 'stats 200': (r) => r.status === 200 });
}

function createOrder() {
  const a = rnd(MAX_ID);
  let b = rnd(MAX_ID);
  if (b === a) {
    b = (a % MAX_ID) + 1;
  }
  const body = JSON.stringify({
    customer: `user-${__VU}`,
    items: [
      { productId: a, quantity: rnd(3) },
      { productId: b, quantity: rnd(3) },
    ],
  });
  const res = http.post(`${BASE_URL}/orders`, body, JSON_HEADERS);
  check(res, { 'order 201 or 409': (r) => r.status === 201 || r.status === 409 });
}

export function shop() {
  const r = Math.random();
  if (r < 0.35) {
    listProducts();
  } else if (r < 0.65) {
    getProduct();
  } else if (r < 0.80) {
    searchProducts();
  } else if (r < 0.85) {
    categoryStats();
  } else {
    createOrder();
  }
}

export function handleSummary(data) {
  const m = data.metrics;
  const dur = m['http_req_duration{phase:load}'].values;
  const count = m['http_reqs{phase:load}'].values.count;
  const failed = m['http_req_failed{phase:load}'].values.rate;
  const r = (x) => Number(x.toFixed(2));

  const result = {
    requests: count,
    rps: r(count / DURATION_S),
    avg: r(dur.avg),
    p90: r(dur['p(90)']),
    p95: r(dur['p(95)']),
    p99: r(dur['p(99)']),
    max: r(dur.max),
    failed: Number(failed.toFixed(4)),
  };

  const text =
    `\n  requests : ${result.requests}\n` +
    `  req/s    : ${result.rps}\n` +
    `  avg      : ${result.avg} ms\n` +
    `  p90      : ${result.p90} ms\n` +
    `  p95      : ${result.p95} ms\n` +
    `  p99      : ${result.p99} ms\n` +
    `  max      : ${result.max} ms\n` +
    `  failed   : ${(result.failed * 100).toFixed(2)} %\n`;

  return {
    [OUT]: JSON.stringify(result, null, 2),
    stdout: text,
  };
}