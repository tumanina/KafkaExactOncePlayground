# KafkaExactOncePlayground

## AtLeastOnce - Outbox transaction

<img width="450" height="400" alt="image" src="https://github.com/user-attachments/assets/bd0b4e75-3f76-4d4b-ba78-f5412c4921e6" />




## How to run

How to run Kafka from docker

`docker run -d -p 9092:9092 --name my-broker apache/kafka:latest`

Console

Create topic
`./kafka-topics.sh --bootstrap-server localhost:9092 --create --topic test-topic`

Run producer
`./kafka-console-producer.sh --bootstrap-server localhost:9092 --topic test-topic`

Send message from shell
<img width="1342" height="371" alt="image" src="https://github.com/user-attachments/assets/5bb9ffef-62ce-4ff8-92a4-3a29ae17ad32" />


UI

Visual code (extension)
https://marketplace.visualstudio.com/items?itemName=Aiven.aiven-kafkaui-vscode-extension

How to connect

<img width="1895" height="798" alt="image" src="https://github.com/user-attachments/assets/2a432c92-e979-400c-bf1f-ee0d60f8dc1b" />

Topics

<img width="1901" height="701" alt="image" src="https://github.com/user-attachments/assets/9d0220f9-5ac9-4b59-8e8b-786562743645" />

Produce message(s)

<img width="1897" height="909" alt="image" src="https://github.com/user-attachments/assets/3cc04a01-ef5e-4bcc-a99e-24ecf32e3507" />

Consume message(s)

<img width="1889" height="879" alt="image" src="https://github.com/user-attachments/assets/c7bd5e01-f41b-4190-b6b5-7020e86b5d08" />


