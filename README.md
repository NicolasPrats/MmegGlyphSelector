# MmegGlyphSelector

## Objectif
Ce projet est un PoC d'un optimisateur automatique de glyphes pour MMEG.

L'objectif est de pouvoir sélectionner des mobs, de donner les glyphes exportées d'un compte et de choisir ce qu'on veut optimiser (la vitesse, les dégâts, ...). L'optimisateur va choisir automatiquement les meilleurs glyphes.

Le projet est **abandonné** car j'ai arrêté de jouer. 

## État d'avancement

Le programme est globalement fonctionnel. Cependant :
* il est complètement inutilisable par un non développeur
* il manque certaines données, notamment les valeurs des dark glyphs : si un fichier avec des dark glyphs est importé, le programme ne pourra pas fonctionner
* il ne gère pas les sets des dark glyphs
* il y a encore 20 millions de petites améliorations à faire

## Fonctionnement global

Le programme permet 2 types d'optimisation :
* Soit la recherche d'un set donnant la vitesse maximale, dans ce cas la réponse est quasiment instantannée et est forcément le résultat optimal (sauf bug). Si plusieurs résultats arrivent à la même valeur, seul l'un d'entre eux est donné.
* Soit la recherche d'un set maximisant une fonction dépendant des statistiques d'un monstre. La fonction peut donc être son effective health ou alors les dégâts de son S3, ... Dans ce cas la réponse donnée est la meilleure trouvée au bout d'un certain nombre d'itérations à l'aide d'un heuristique. Le temps mis dépend de la machine utilisée.

Le fichier Program.cs montre les optimisations que j'ai utilisée pour glypher ma team arène offensive :
* Croisé vent le plus rapide possible
* Dragon de glace le plus rapide possible avec au moins 100 de précision (mais sans utiliser les glyphes du croisé)
* Chaman terre le plus rapide possible  (mais sans utiliser les glyphes du croisé et du dragon)
* Rak air qui fait le plus de dégâts possible sur son S3 (sans utiliser les glyphes des autres mobs)

A noter que j'ai demandé à optimiser le dragon avant le chamy car je savais que le chamy serait quand même plus rapide à cause de la contrainte imposée au dragon.

## Fonctionnement de l'heuristique

En gros, il fonctionne en plusieurs passes :
1. Il détermine une valeur pour chaque glyphe à l'aide d'un algorithme à mi chemin entre du [Monte-Carlo](https://fr.wikipedia.org/wiki/Algorithme_de_Monte-Carlo) et de l'[algorithme des fourmis](https://fr.wikipedia.org/wiki/Algorithme_de_colonies_de_fourmis)
1. Au bout d'un moment, il élimine la moitié des glyphes les moints intéressants puis continue avec le même algorithme pour raffiner la valeur
1. Au bout d'un moment, il ne garde que les glyphes les plus intéressants. A partir de là, il va faire une recherche exhaustive sur toutes les combinaisons possibles

Il y a quelques raffinements pour bien prendre en compte les sets.
A noter qe pouor qu'il fonctionne correctement l'heuristique a besoin que la fonction à optimiser ne fonctionne pas par palier et soit globalement croissante en fonction des statistiques des glyphes. Ce dernier point n'est pas défini très rigoureusement mais globalement si 1 combinaison de glyphe semble meilleure qu'une autre, il faut que la fonction renvoie une plus grande valeur même si les 2 combinaisons sont nulles.


