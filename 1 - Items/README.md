# kafka-dotnet
PoC de Kafka com resiliência no consumo de mensagens

## Apresentação

Este repositório tem como objetivo apresentar o mecanismo de resiliência após consumo de uma mensagem no Confluent Kafka.

Ao invés de utilizar outras tecnologias como filas para criar uma resiliência no processamento pós consumo de mensagens, abordo aqui o mecanismo de avanço do offset apenas ao final do processamento com sucesso, caso haja qualquer problema o offset não é avançado, permitindo processar novamente a mesma mensagem em consumo posterior oportuno da mensagem.

Para isto divido esta PoC em algumas etapas, sendo:

1. Subir um cluster Kafka simulando 3 nós no Docker, definindo ao menos 6 partições. Para isto é necessário que você tenha o Docker instalado
2. Criação de mensagens através de um aplicativo console em .Net, específico para atuar como producer
3. Leitura de mensagens através de um aplicativo console em .Net, específico para atuar como consumer
4. Pontos importantes dessa PoC

Referência: [https://docs.confluent.io/kafka-clients/dotnet/current/overview.html](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)

## Subida do cluster Kafka

Utilize a definição do script Docker Compose presente na pasta "kafka-cluster" e o seguinte comando para subir o cluster Kafka com 3 nós (lembre-se de executar este comando dentro da pasta "kafka-cluster"):

`docker compose up -d`

Se você não alterou nada do que está no docker-compose.yml, o cluster deverá subir com os seguintes containers:

- zookeeper1 (na porta 22181)
- zookeeper2 (na porta 23181)
- kafka1 (na porta 24092 para acesso via rede interna e na porta 24192 para acesso via localhost)
- kafka2 (na porta 25092 para acesso via rede interna e na porta 25192 para acesso via localhost)
- kafka3 (na porta 26092 para acesso via rede interna e na porta 26192 para acesso via localhost)
- kafka-ui (na porta 27080)

Todos esses containers estarão na mesma rede, chamada "kafka-network". Isso é necessário para que os containers "se conversem".

Você poderá abrir o kafka-ui que é uma interface para gerenciar os tópicos colocando o seguinte endereço no browser (Google Chrome, por exemplo): localhost: 27080

Para os testes que faremos à seguir, peço criar um tópico chamado "teste" (sem as aspas duplas) com 6 partições, o restante das configurações pode manter os defaults.

## Producer

Na pasta "kafka-producer" há uma aplicação em .Net Core com uma interface de API para que você consiga postar quantas mensagens quer inserir no tópico "teste".

Nesta pasta há uma collection do Postman pronta para que você possa fazer esta requisição, o nome da collection é: "kafka-dotnet.postman_collection.json".

Importante: caso você mude o nome do tópico ou o nome dos nós do Kafka, atente-se a atualizar o appsettings.json e appsettings.Development.json, pois é lá que estas informações estão parametrizadas.

Este projeto é muito simples, ele gera um GUID novo para cada mensagem, só para testarmos a inclusão de qualquer coisa no tópico.

Não é garantido que o Kafka consiga fazer o round-robin das mensagens para as partições, pois é o Kafka quem gerencia. Não há nenhuma configuração na aplicação para jogar tudo para uma única partição.

Se você quiser subir esta aplicação no Docker também, você pode fazer o build com o seguinte comando (esteja dentro da pasta "kafka-producer"):

`docker build -t evertonjuniti/kafka-producer:latest .`

Se quiser mexer em algo na aplicação, é só excluir a imagem e gerar outro build. É fácil!

E subir um container simples dessa imagem com o seguinte comando:

`docker run --name kafka-producer --network kafka-network -p 5000:80 -d evertonjuniti/kafka-producer:latest`

Por default é exposta a porta 80 da aplicação, pois eu não indiquei expor outra porta no Dockerfile, mas você pode mudar isso, só terá que apagar a imagem recém criada e fazer um novo build.

## Consumer

Na pasta "kafka-consumer" há uma aplicação em .Net Core que é um worker service simples que fica consumindo mensagens do tópico "teste".

Importante: caso você mude o nome do tópico ou o nome dos nós do Kafka, atente-se a atualizar o appsettings.json e appsettings.Development.json, pois é lá que estas informações estão parametrizadas.

Este projeto é muito simples, ele vai consumindo as mensagens do tópico e propositadamente gera uma Exception a cada 5 leituras de mensagens. Isso é proposital para que você veja que, desde que você não atualize o controle de offset, a mensagem poderá ser lida novamente pois o offset daquela partição não se mexeu.

Essa é a resiliência necessária para que, em caso de algum problema na sua aplicação, que você consiga ler novamente as mensagens, sem a necessidade de uso de alguma tecnologia extra para isso (como uma fila,por exemplo).

Se você quiser subir esta aplicação no Docker também, você pode fazer o build com o seguinte comando (esteja dentro da pasta "kafka-consumer"):

`docker build -t evertonjuniti/kafka-consumer:latest .`

Se quiser mexer em algo na aplicação, é só excluir a imagem e gerar outro build. É fácil!

Eu deixei criado um arquivo Docker Compose nesta pasta que cria 3 containers dessa imagem recém criada. Você pode mudar o docker-compose.yml para incluir mais imagens, só tome cuidado com o nome dos containers.

`docker compose up -d`

## Pontos importantes

Obviamente que você poderá implementar as estratégias que julgar necessário para implementar a resiliência da sua aplicação, mas aqui é como se você estivesse dando um commit a cada rodada com sucesso do fluxo por inteiro que sua aplicação venha a fazer.

Para isto é importante que você lembre da seguinte parametrização na configuração do seu Consumer Kafka:

`EnableAutoOffsetStore = false`

É aqui que está a mágica! Você vai incluindo no que chamamos de OffsetStore as mensagens que podem ser marcadas para serem "commitadas", isso fará com que o offset "mova para frente", assim você não lerá a mesma mensagem.

Para que você use a OffsetStore para indicar o commit (que não é feito na hora, nós incluímos a mensagem nesse OffsetStore para ela ser marcada para commit num momento futuro), preste atenção na seguinte linha de código:

`consumer.StoreOffset(resposta);`

Lembre-se que a variável "resposta" é a mensagem consumida do tópico, no seu caso mude para a sua varíavel.

Essa linha deveria estar no final de todo o fluxo da sua aplicação, antes de tentar consumir a próxima mensagem, é como se fosse um "commit" de um fluxo "transacional" comum, nada de novidade até aqui.

Outro ponto importante é que no meu exemplo eu decidir "fechar" o consumer, mas você pode controlar tudo isso da forma que achar melhor. Mas eu acredito que da forma que eu fiz, apesar de bastante "artesanal", replica o comportamento que haveria na produção, ou seja, algo não esperado acontece e você encerra o consumer, faz alguma outra coisa (log, por exemplo) e abre o consumer novamente para tentar mais uma vez (depois de x segundos, talvez).

### Conclusão

Com apenas 2 linhas de código, você evita um overengineering da sua solução (evita-se incluir outros mecanismos como filas, cache, etc) e deixa para a própria aplicação a gestão do offset para maior controle de quando ir pra frente ou não.