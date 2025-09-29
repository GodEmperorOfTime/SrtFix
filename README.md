# SrtFix

## Ucel

Jednoduche serizeni casovani titulku. tzn.:
  * Posunuti vsech titulku o konstantni cas (`shift`)
  * Vynasobeni vsech casu konstantou (`scale`)

Parsovani by melo byt odolne vuci zprasenym souborum.

Ucelem neni nastroj na komplexni editaci titulku.

## TODO

  * Pozor na kodovani parsovanych srt souboru (asi doplnit nejakou detekci kodovani)
  * Error handling:
    * Hadam, ze to vyhodi nejakou divnou chybovku, pokud nebude uveden soubor
    * Lepsi popis chybovek, kdyz spadne parsovani souboru
  * Dusledne pouzivat `CancellationTokeny`

## Napady

  * Mozna umoznit specifikovat kodovani ukladaneho souboru
  * mozna nejaky config kde by byly vychozi hodnoty (kodovani nebo neco podobneho)