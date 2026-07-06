# Dockerfile

FROM rabbitmq:3.7-management

COPY rabbitmq.conf /etc/rabbitmq/rabbitmq.conf
COPY definitions.json /etc/rabbitmq/definitions.json

RUN cat /etc/rabbitmq/rabbitmq.conf