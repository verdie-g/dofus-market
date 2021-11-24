# Dofus Market

Bot to monitor item prices of all [Dofus 2](https://www.dofus.com) servers. The bot was
very loud so it quickly got banned but it was fun to do 🤷‍♂️

The bot written in .NET would scrap the item prices from auction houses, send them to an
ASP.NET Core app that would write them into a TimescaleDB and exposed in a Grafana.

![dofus-market](https://user-images.githubusercontent.com/9092290/143322483-130585a8-45bb-40ff-8aac-efc4d54b6c63.png)
