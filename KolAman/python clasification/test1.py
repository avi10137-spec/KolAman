



import json
import datetime
import pika


class AppLogger:
    def __init__(self, service_name="ClassifierService"):
        self.service_name = service_name

    def info(self, message: str):
        self._write_log("INFO", message)

    def warning(self, message: str):
        self._write_log("WARNING", message)

    def error(self, message: str):
        self._write_log("ERROR", message)

    def _write_log(self, level: str, message: str):
        timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        print(f"[{timestamp}] [{self.service_name}] [{level}] - {message}")


logger = AppLogger("ClassifierService")

connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()

EXCHANGE_NAME = 'command_alerts'
channel.exchange_declare(exchange=EXCHANGE_NAME, exchange_type='direct', durable=True)


def send_to_rabbit(command_name: str, alert_data: dict):
    message_body = json.dumps(alert_data)

    channel.basic_publish(
        exchange=EXCHANGE_NAME,
        routing_key=command_name,
        body=message_body,
        properties=pika.BasicProperties(
            delivery_mode=2
        )
    )

    logger.info(f"Alert {alert_data.get('id')} sent successfully to command: {command_name}")


alert = {
    "id": "ALT-9876",
    "lat": 32.0853,
    "lon": 34.7818,
    "timestamp": "2026-10-05T05:40:00"
}

assigned_command = classify_location(alert["lat"], alert["lon"])
send_to_rabbit(assigned_command, alert)

connection.close()




