# Dofus Market

Second attempt to write a bot to monitor item prices of all [Dofus 2](https://www.dofus.com) servers.

The first attempt was a complete implementation of the Dofus protocol to connect to the servers without
the official client. It could therefore behave different from the official client, making it easy to
detect. The code is still available in the [bot-socket branch](https://github.com/verdie-g/dofus-market/tree/bot-socket).

This bot is sniffing packets from the Dofus servers using [npcap](https://npcap.com) and injects user
inputs to the Dofus client window using [win32's winuser.h](https://learn.microsoft.com/en-us/windows/win32/api/winuser).
Packets containing item prices are sent to a Grafana stack (Grafana, Mimir, Loki) at [dofus-market.com](https://dofus-market.com).

![result in grafana](https://github.com/verdie-g/dofus-market/assets/9092290/70493a29-64a8-4e0f-9e21-cb0199e0d356)

I discontinued the project because I have no more interest in Dofus.
