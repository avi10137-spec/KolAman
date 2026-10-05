
import pika
import json

connection = pika.BlockingConnection(
    pika.ConnectionParameters(host="localhost")
)

channel = connection.channel()

channel.exchange_declare(
    exchange="location_exchange",
    exchange_type="topic",
    durable=True
)

alert = {
    "id": "12345",
    "latitude": 32.0853,
    "longitude": 34.7818,
    "message": "Alert"
}

latitude = alert["latitude"]
longitude = alert["longitude"]

routing_key = f"location.{latitude}.{longitude}"

channel.basic_publish(
    exchange="location_exchange",
    routing_key=routing_key,
    body=json.dumps(alert),
    properties=pika.BasicProperties(
        content_type="application/json"
    )
)

print(f"Message sent: {routing_key}")





