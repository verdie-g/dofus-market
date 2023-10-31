# Dofus Market

Second attempt to write a bot to monitor item prices of all [Dofus 2](https://www.dofus.com) servers.

The first attempt was a complete implementation of the Dofus protocol to connect to the servers without
the official client. It could therefore behave different from the official client, making it easy to
detect. The code is still available in the [bot-socket branch](https://github.com/verdie-g/dofus-market/tree/bot-socket).

This bot is sniffing packets from the Dofus servers using [npcap](https://npcap.com) and injects user
inputs to the Dofus window using [win32's winuser.h](https://learn.microsoft.com/en-us/windows/win32/api/winuser).
Packets containing item prices are sent to a Grafana stack (Grafana, Mimir, ...) at [dofus-market.com](https://dofus-market.com).

The problem is that there are tens of thousands of items and ~ten Dofus server so I believe sending a million of packet
a day will quickly get it ban for failing the behavioral tests from Ankama. On top of that, Ankama just announced that
they included [Epic's easy anti-cheat](https://www.easy.ac) in the next version of Dofus. So we won't be able to get
item price trends anytime soon.
