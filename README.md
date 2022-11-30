# [Szpek](https://szpek.pl/) 
[![GitHub publish-workflow-status](https://github.com/wojciechN9/Szpek/actions/workflows/publish.yml/badge.svg)](https://github.com/wojciechN9/Szpek/actions/workflows/publish.yml/)
[![GitHub release-date](https://img.shields.io/github/release-date/wojciechN9/Szpek)](https://github.com/wojciechN9/Szpek) 
[![GitHub release](https://img.shields.io/github/v/release/wojciechN9/Szpek)](https://github.com/wojciechN9/Szpek) 
[![GitHub stars](https://img.shields.io/github/stars/wojciechN9/Szpek)](https://github.com/wojciechN9/Szpek/stargazers)

<!-- TODO add badges after creating git pipeline: .net, test coverage (integration, unit) -->



**Szpek** is an air pollution sensors farm based on microcontrollers which displays the results on the website. 

The air quality sensor takes several hundred measurements per hour, which are then averaged and sent to a server.
The sensors take measurements of PM10, PM2.5, PM1 dust. They also contain data from which the pressure and humidity of the air is calculated.

<a href="https://szpek.pl">![logo](Assets/meassurement-photo.png?raw=true)</a>

**Website:** https://szpek.pl

## Features

* **Dashboard** - displays up-to-date pollution data from all locations
* **Details page** - shows detailed data from the last 24 hours 
* **Map** - shows location of sensors with pollution information
* **Admin panel** - adding new users, modifying sensor locations, admin stuff
* **User panel** - managing your sensor, access to data from historical meassurements

## How it looks
| ![logo](Assets/sensor.jpg?raw=true) | 
|:--:| 
| *Sensor in operation* |

## Getting started
TODO: create docker compose
might environment variables be needed

## API

* [Szpek Swagger API](https://api.szpek.pl/api/)
* [Szpek UI APP](https://github.com/wojciechN9/Szpek-UI)

## Contributing 

Szpek is an open source project and you are welcome to participate - just let us know or create a pull requests.

## License

Szpek is released under the [The Commons Clause License](https://github.com/wojciechN9/Szpek/blob/master/LICENSE).

**Sensor designed and manufactured by Szymon Katra** 

**Project created by Wojciech Nastaj**
