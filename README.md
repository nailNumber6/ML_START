Ветка v.2.0 - основная. Возможно это не очень правильно, но я поменял ветку по умолчанию, так как со старой было сложнее работать (из-за синхронизации с сабмодулями, доп. коммитов и последующих ошибок).
1) Точки входа: 2 (Server, ML_START_GUI) - После авторизации открывается окно клиента (как я понял, нужно сделать наоборот, в скором времени исправлю)
Примечание: В консольном проекте (ML_START_1) пока находятся все элементы из первого и второго заданий, а также метод Main, где всё это выполняется. В скором времени я полностью перенесу их на сервер.
3) Пути к файлам логов: C:\VS Projects\ML_START_1\ML_START_1\bin\Debug\net7.0\log, C:\VS Projects\ML_START_1\ML_START_GUI\bin\Debug\net7.0\log  
Примечание: логирование сервера и клиента еще не реализованы
4) Пути к файлам конфигурации (2 исполняемых файла - 2 конфига): C:\VS Projects\ML_START_1\ML_START_1\bin\Debug\net7.0, C:\VS Projects\ML_START_1\ML_START_GUI\bin\Debug\net7.0  
5) ОС - Windows 10  
6) Avalonia UI  
7) .NET 7.0  
8) Все задания из файла 1,2 реализованы; из задания 3 - графический интерфейс, хэширование пароля, авторизация пользователя, логирование, окна клиента и сервера, на сервере асинхронно выводится история.
