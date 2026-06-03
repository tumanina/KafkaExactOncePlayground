# KafkaExactOncePlayground

In distributed systems, achieving true exactly-once message processing is challenging because network failures, service crashes, and retries can cause messages to be delivered never or more than once. A common issue is the dual-write problem, where a service updates its database successfully but fails to publish the corresponding event, leaving the system in an inconsistent state. 

The Outbox and Inbox patterns address these challenges by ensuring reliable event delivery and idempotent processing, providing effectively-once semantics from a business perspective.



<img width="741" height="317" alt="image" src="https://github.com/user-attachments/assets/0b108e5b-2c2a-431e-8b54-d38e1f94ebf5" />


## AtLeastOnce - Transactional Outbox

- Should contain eventId as IdempotencyKey
- Should have version of event
- CorrelationId for tracing
- Cleaner to remove "old" events in database
- Good to have partion per agrregation root (UserId in this case)

## AtMostOnce - Transactional Inbox 

- Events should be idempotent base on IdempotencyKey: check in logic or make property Unique.
- Listener should be single (by Mutex for example) or block rows of table that in process
- Event should be deserialized base on version



## How to run

How to run Kafka from docker

`docker run -d -p 9092:9092 --name my-broker apache/kafka:latest`

Console

Create topic
`./kafka-topics.sh --bootstrap-server localhost:9092 --create --topic test-topic`

Run producer
`./kafka-console-producer.sh --bootstrap-server localhost:9092 --topic test-topic`

Send message from shell

<img width="671" height="170" alt="image" src="https://github.com/user-attachments/assets/5bb9ffef-62ce-4ff8-92a4-3a29ae17ad32" />


UI

Visual code (extension)
https://marketplace.visualstudio.com/items?itemName=Aiven.aiven-kafkaui-vscode-extension

How to connect

<img width="950" height="400" alt="image" src="https://github.com/user-attachments/assets/2a432c92-e979-400c-bf1f-ee0d60f8dc1b" />

Topics

<img width="950" height="350" alt="image" src="https://github.com/user-attachments/assets/9d0220f9-5ac9-4b59-8e8b-786562743645" />

Produce message(s)

<img width="950" height="450" alt="image" src="https://github.com/user-attachments/assets/3cc04a01-ef5e-4bcc-a99e-24ecf32e3507" />

Consume message(s)

<img width="950" height="440" alt="image" src="https://github.com/user-attachments/assets/c7bd5e01-f41b-4190-b6b5-7020e86b5d08" />


