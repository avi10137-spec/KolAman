from classification import *
from logger_ import ElasticLogger
from confluent_kafka import Consumer
import redis
import json
import  time

def validate_alert(alert):
    required_fields = ["alert_id", "source", "title","content","priority","classification","lat","lon","timestamp","status"]

    for field in required_fields:
        if field not in alert or alert[field] is None:
            return False, f"Missing field: {field}"


    if not isinstance(alert["alert_id"], (str, int)) or str(alert["alert_id"]).strip() == "":
        return False, "Invalid id"


    if not isinstance(alert["lat"], (int, float)):
        return False, "Invalid latitude"

    if not -90 <= alert["lat"] <= 90:
        return False, "Latitude out of range"

    if not isinstance(alert["lon"], (int, float)):
        return False, "Invalid longitude"

    if not -180 <= alert["lon"] <= 180:
        return False, "Longitude out of range"

    return True, "Valid"
def create_kafka_consumer():
    conf = {
        'bootstrap.servers': 'localhost:9092',
        'group.id': 'classifier-group15',
        'auto.offset.reset': 'earliest'
    }
    consumer = Consumer(conf)
    consumer.subscribe(['notifications'])
    return consumer

def create_redis_client():
    redis_client = redis.Redis(
        host="localhost",
        port=6379,
        decode_responses=True
    )
    return redis_client
def consum_from_kafka():
    consumer = create_kafka_consumer()
    redis_client = create_redis_client()
    timeout_seconds = 30
    last_message_time = time.time()

    try:
        while True:
            msg = consumer.poll(1.0)
            print(msg)
            if msg is None:
                if time.time() - last_message_time >= timeout_seconds:
                    print(f"No messages received for {timeout_seconds} seconds")
                    print("Stopping consumer...")
                    break

                logger.warning("msg is none")
                continue


            if msg.error():
                print(f"Kafka error: {msg.error()}")
                logger.error(f"Kafka error: {msg.error()}")
                continue
            try:
                alert = json.loads(msg.value().decode("utf-8"))
                print(alert)
            except json.JSONDecodeError:
                print("Invalid JSON")
                logger.error("Invalid json")
                continue
            else:
                is_valid, message = validate_alert(alert)
                if is_valid:
                    print("Valid alert")
                    # print(alert)
                    alert_id = alert["alert_id"]
                    # print(alert_id)
                    if redis_client.exists(f"alert:{alert_id}"):
                        print(f"Duplicate alert: {alert_id}")
                        logger.info(f"Duplicate alert: {alert_id}")
                        continue
                    redis_client.setex(
                        f"alert:{alert_id}",
                        3600,
                        "1"
                    )

                    # print(f"New alert: {alert_id}")
                    # print(alert)
                    process_alert(alert)
                    logger.info("send alert from rabbit")
                else:
                    print(f"Invalid alert: {message}")
                    logger.warning(f"Invalid alert: {message}")
                    continue
    finally:
        consumer.close()
logger = ElasticLogger(index_name="clasification-logs")
if __name__ == "__main__":
    consum_from_kafka()


