# FactoryAPI
## Требования
### Общие
- Docker

    Для Windows: https://docs.docker.com/docker-for-windows/install/

    Для Ubuntu: https://docs.docker.com/engine/install/ubuntu/

### Для разработки
- WIndows 10
- Visual Studio 2019: https://visualstudio.microsoft.com/ru/downloads/
- Аккаунт на https://hub.docker.com/

## Компиляция и запуск в режиме отладки

1. Установить Docker for Windows.
2. Загрузить код из репозитария.
3. Открыть решение в Visual Studio 2019.
4. Выбрать в качестве запускаемого проекта docker-compose.
5. Запустить отладку нажатием F5.
6. Можно посылать запросы по адресу http://localhost:50000/
7. Для обновления образа на DockerHub нужно остановить отладку, в меню зайти в Build -> Publish, настроить публикацию на Docker Hub, нажать кнопку Publish.

## Запуск на Ubuntu

1. Установить Docker
2. Загрузить образ Docker-контейнера:

        docker pull dimanoid2018/factoryapi

3. Запустить контейнер из образа: 

        docker run dimanoid2018/factoryapi