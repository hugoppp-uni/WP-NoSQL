# Termin 1 Dokumentation
## Aufgabe 4
Die Aufgabe wurde in Form einer REST-API gelöst.
Die Dokumentation dieser ist unter Anderem mithilfe von Swagger auf Port `31474` (`http://localhost:31474/swagger`) vorzufinden.

Es existieren zwei Endpoints, welche im `PlzDataController` defeniert sind:
 1. `GET /city/{zipCode}` gibt die den Stadtnamen und Staat der Postleitzahl zurück.
 2. `GET /zip/{cityName}` gibt eine Liste aller Postleizahlen züruck, dessen Stadtnahme `cityName` entspricht.

In der Redis Datenbank werden dafür die folgenden Daten importiert:

1. Key: `{zip}.state` Value: Der Staat, in dem sich die Stadt mit der Postleitzahl `{zip}` befindet
2. Key: `{zip}.city` Value: Der Name der Stadt mit der Postleitzahl `{zip}`
3. Key: `{city}.plz` Value: Eine Liste aller Postleitzahlen mit dem Stadtnamen `{city}`

## Aufgabe 5

## Aufgabe 6
Die Cypher-Anfrage, die für den Knoten mit der ID `/f/en/baseball` alle direkt mit dem
Kantenlabel „IsA“ verbundenen Knoten lautet:
```cypher
Match (baseball{id:"/c/en/baseball"})-[isA:IsA]->(n)
return n
```


