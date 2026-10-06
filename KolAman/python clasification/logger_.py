

import os
from elasticsearch8 import Elasticsearch
import json
from datetime import *
import pika




class ElasticLogger:
    def __init__(self, index_name: str = "clasification-logs", es_host: str = None):

        host = es_host or os.getenv("ELASTICSEARCH_HOST", "http://localhost:9200")
        self.es = Elasticsearch(host)
        self.index_name = index_name
    def _send_log(self, level: str, message: str):

        log_entry = {
            "timestamp": datetime.now(timezone.utc).isoformat(),
            "level": level,
            "message": message,
        }
        try:

            self.es.index(index=self.index_name, document=log_entry)
        except Exception as e:
            print(f"[Logging Error] Failed to send log to Elasticsearch: {e}")


    def info(self, message: str):
        self._send_log("INFO", message)

    def warning(self, message: str):
        self._send_log("WARNING", message)

    def error(self, message: str):
        self._send_log("ERROR", message)


logger = ElasticLogger(index_name="clasification-logs")























# def send_event_example(event_data):
#     try:
#
#         logger.info("Attempting to send event", {"event_id": event_data.get("event_id")})
#
#
#         if not event_data.get("value"):
#             logger.warning("Event contains empty value", {"event": event_data})
#
#
#         logger.info("Event successfully sent", {"event_id": event_data.get("event_id")})
#
#     except Exception as e:
#
#         logger.error("Failed to send event", {
#             "event_id": event_data.get("event_id"),
#             "error": str(e)
#         })




