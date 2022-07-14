# POC Arquitetura de consumo de eventos
PoC com diferentes cases de consumo de eventos do Kafka

## Requisitos Básicos
* SDK Microsoft .Net 5
* Docker
* Subir Cluster kafka
* Subir API publicadora
* Subir Cluster RabbitMQ

## Apresentação
Este repositório tem como objetivo apresentar 6 cases de consumo de eventos do Confluent Kafka onde temos diferentes maneiras de lidar com os eventos enviados para um tópico, com isso podemos demonstrar a eficiencia de cada solução para o cenário de produto propost.
Dentro das aplicações de consumo do kafka, utilizamos a mudança de offset como forma de dar ACK.

### Observações de ambiente:
* Manter as portas das aplicações que já estão setadas nos arquivos de configurações, facilitará muito o seu trabalho de organização do ambiente. Caso deseje alterart, lembre-se de alterar as aplicações que consomem esses caminhos.
* As imagens estão com prefixo "wmaschette" por padrão, caso queira personaliza-las ao seu modo, lembre-se de alterar nos respectivos arquivos docker-compose e appsettings das aplicações que consomem o recurso.
* Todos os containers estaram na mesma rede chamada "kafka-network", para que consigam se conversar de maneira igual.
* Todos os cases estarão separados por pastas no caminho "./3 - Cases/CaseN"
* Para subir uma nova imagem, sempre exclua a anterior e gere um novo build


## Subida do cluster Kafka
Utilize a definição do script Docker Compose presente na pasta "1 - Items/kafka-cluster" e o seguinte comando para subir o cluster Kafka com 3 nós (lembre-se de executar este comando dentro da pasta "kafka-cluster"):

`docker compose up -d`

O cluster deverá subir com os seguintes containers:

- zookeeper1 (na porta 22181)
- zookeeper2 (na porta 23181)
- kafka1 (na porta 24092 para acesso via rede interna e na porta 24192 para acesso via localhost)
- kafka2 (na porta 25092 para acesso via rede interna e na porta 25192 para acesso via localhost)
- kafka3 (na porta 26092 para acesso via rede interna e na porta 26192 para acesso via localhost)
- kafka-ui (na porta 27080)

Você poderá abrir o kafka-ui que é uma interface para gerenciar os tópicos colocando o seguinte endereço no browser (Google Chrome, por exemplo): localhost: 27080

Para realizar um teste simples, deve-se criar um tópico com 1 partição, mantendo as configurações default do tópico.
Também não haverá problema caso queira realizar um teste com mais partições.

## Producer

Na pasta "2 - Common/Producer/kafka-producer" há uma aplicação em .Net 5 com uma interface de API para que você consiga postar quantas mensagens quer inserir no tópico "teste".

Nesta pasta há uma collection do Postman pronta para que você possa fazer esta requisição, o nome da collection é: "kafka-dotnet.postman_collection.json".

Este projeto é muito simples, ele gera um GUID novo para cada mensagem, só para testarmos a inclusão de qualquer coisa no tópico.

Não é garantido que o Kafka consiga fazer o round-robin das mensagens para as partições, pois é o Kafka quem gerencia. Não há nenhuma configuração na aplicação para jogar tudo para uma única partição.

Para subir esta aplicação no Docker também, você pode fazer o build com o seguinte comando (esteja dentro da pasta "2 - Commom/Producer/kafka-producer"):

`docker build -t wmaschette/kafka-producer:latest .`

Para subir um container simples execute o comando (esteja dentro da pasta "2 - Commom/Producer/kafka-producer"):

`docker compose up -d`

## Case 1
Caminho: "./3 - Cases/Case1"

1. Na pasta "kafka-consumer" há uma aplicação worker em .Net 5 subscrita no tópico principal contendo uma porta listener e toda regra de negócio específica ao domínio.
![image-case1](./3 - Cases/Case1/Items/case1.png)

Para subir esta aplicação no Docker, você pode fazer o build com o seguinte comando (esteja dentro da pasta "kafka-consumer"):

- Dentro da pasta "kafka-consumer"`docker build -t wmaschette/kafka-consumer-doamin:latest .`

Para subir um container simples execute o comando:

- Dentro da pasta Case1 >> `docker compose up -d`

## Case 2
Caminho: "./3 - Cases/Case2"

1. Na pasta "kafka-consumer" há uma aplicação worker em .Net com porta Listener, sem regra de negócio e com uma chamada REST API
2. Na pasta "api-domain" há uma aplicação com porta REST contendo toda regra de negócio específica ao domínio.
![image-case2](./3 - Cases/Case2/Items/case2.png)

Se você quiser subir estas aplicações no Docker, você pode fazer o build com o seguinte comando:

- Dentro da pasta "kafka-consumer" >> `docker build -t wmaschette/kafka-consumer-request:latest .`
- Dentro da pasta "api-domain" >> `docker build -t wmaschette/api-domain:latest .`

E para subir os containers, execute o comando:

- Dentro da pasta Case2 >> `docker compose up -d`

## Case 3
Caminho: "./3 - Cases/Case3"

1. Na pasta "adapter-architecture" há uma aplicação com 2 portas (REST + Listener), contendo toda regra de negócio específica ao domínio.
2. Na pasta "kafka-consumer" há uma aplicação consumer para realizar chamadas fake para porta Rest da aplicação principal.
![image-case3](./3 - Cases/Case3/Items/case3.png)

Se você quiser subir estas aplicações no Docker, você pode fazer o build com o seguinte comando:

- Dentro da pasta "adapter-architecture" >> `docker build -t wmaschette/api-consumer:latest .`
- Dentro da pasta "kafka-consumer" >> `docker build -t wmaschette/kafka-consumer-fake:latest .`

E para subir os containers, execute o comando:

- Dentro da pasta Case3 >> `docker compose up -d`

## Case 4
Caminho: "./3 - Cases/Case4"

1. Na pasta "kafka-consumer" há uma aplicação com porta listener para o tópico kafka e publicando em uma fila RabbitMQ
2. Na pasta "rabbit-consumer" há uma aplicação listener da fila RabbitMQ e realizando chamada REST para API de domínio.
3. Na pasta "api-domain" há uma aplicação com porta REST e contendo toda regra de negócio específica ao domínio.
![image-case4](./3 - Cases/Case4/Items/case4.png)

Se você quiser subir estas aplicações no Docker, você pode fazer o build com o seguinte comando:

- Dentro da pasta "api-domain" >> `docker build -t wmaschette/api-domain-rabbit:latest .`
- Dentro da pasta "kafka-consumer" >> `docker build -t wmaschette/kafka-consumer-producer:latest .`
- Dentro da pasta "rabbit-consumer" >> `docker build -t wmaschette/rabbit-consumer:latest .`

E para subir os containers, execute o comando:

- Dentro da pasta Case4 >> `docker compose up -d`

## Case 5
Caminho: "./3 - Cases/Case5"

1. Na pasta "adapter-architecture" há uma aplicação com porta Rest e Listener, contendo toda regra de negócio específica ao domínio e uma estratégia de retentativa utilizando fila em RabbitMQ.
2. Na pasta "rabbit-consumer" há uma aplicação listener da fila RabbitMQ e realizando chamada REST para API de domínio.
![image-case5](./3 - Cases/Case5/Items/case5.png)

Se você quiser subir estas aplicações no Docker, você pode fazer o build com o seguinte comando:

- Dentro da pasta "adapter-architecture" >> `docker build -t wmaschette/consumer-api-retry:latest .`
- Dentro da pasta "rabbit-consumer" >> `docker build -t wmaschette/rabbit-consumer-retry:latest .`

E para subir os containers, execute o comando:

- Dentro da pasta Case5 >> `docker compose up -d`

## Case 6
Caminho: "./3 - Cases/Case6"

1. Na pasta "adapter-architecture" há uma aplicação contendo porta uma REST e duas portas Listener (uma para o tópico e outra para a fila), contendo toda regra de negócio específica ao domínio e uma estratégia de retentativa utilizando fila em RabbitMQ. 
![image-case6](./3 - Cases/Case6/Items/case6.png)

Se você quiser subir estas aplicações no Docker, você pode fazer o build com o seguinte comando:

- Dentro da pasta "adapter-architecture" >> `docker build -t wmaschette/adapters-application:latest .`

E para subir os containers, execute o comando:

- Dentro da pasta Case6 >> `docker compose up -d`


## Pontos importantes
A POC não tem a intenção de demonstrar a melhor maneira de realizar a resiliência dos processos, pois cada aplicação deve ser responsável pela sua política de resiliencia e retentativa.
Cada aplicação representa um conteiner docker, porém um case não representa o número de pods.