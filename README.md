# Чатик в качестве проекта? Ну да, сделал Русаков Владислав

Небольшое клиент-серверное приложение, созданное с применением MVVM паттерна, ассинхронных сокетов и бд MySQL
<br />
Сервер управляет подключениями, исполнением sql запросов к базе

## Что он умеет?
Регистрация с email-подтверждением. Юзеру отправляется зашифрованный код активации, введя который на клиенте - можно подтверить регистрацию <br />
Сообщения: <br />
* Приватные сообщения
* Сообщения для всех
<br />
Так же есть функционал по добавлению/удалению друзей

## Сборка

Вам понадобится:
* Visual Studio v15(2017)
* MySql server (локальный или удаленный)

Просто запустить Chat.sln and скомпилировать сервер и клиент. Так же можно собрать приложения и запускать уже через exe-шники
<br />
Так же необходимо:
* импортнуть chat.sql на ваш бд-сервер
* отредактировать Server/Settings.cs для настройки подключения к вашей бд
