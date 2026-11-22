FROM python:3.12-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY app ./app

ENV PYTHONPATH=/app
ENV WEB_CONCURRENCY=2

EXPOSE 8082

CMD ["sh", "-c", "exec uvicorn app.main:app --host 0.0.0.0 --port 8082 --workers ${WEB_CONCURRENCY}"]