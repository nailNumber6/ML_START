На данный момент самый стабильный коммит v2.5a, но в нем я еще не перенес модель БД в проект Server
Ветка v.2.0 - основная. Возможно это не очень правильно, но я поменял ветку по умолчанию, так как со старой было сложнее работать (из-за синхронизации с сабмодулями, доп. коммитов и последующих ошибок). 
Для создания БД можно в package manager консоли запустить команду Update-Database InitMigrationAttempt1000.

1) Точки входа: 2 (Server, ML_START_GUI) - После авторизации открывается окно клиента (как я понял, нужно сделать наоборот, в скором времени исправлю)
Примечание: В консольном проекте (ML_START_1) пока находятся все элементы из первого и второго заданий, а также метод Main, где всё это выполняется. В скором времени я полностью перенесу их на сервер.

2) Пути к файлам логов: C:\VS Projects\ML_START\ML_START_GUI\bin\Debug\net7.0\log, C:\VS Projects\ML_START\Server\bin\Debug\net7.0\log
Примечание: логирование сервера и клиента еще не реализованы  
3) Пути к файлам конфигурации: C:\VS Projects\ML_START\Server\bin\Debug\net7.0  
4) ОС - Windows 10    
5) Avalonia UI   
6) .NET 7.0
7) СУБД - MSSQL Server    
8) Все задания из файла 1,2 реализованы; из задания 3 - графический интерфейс, хэширование пароля, авторизация пользователя, логирование, окна клиента и сервера, на сервере асинхронно выводится история.
