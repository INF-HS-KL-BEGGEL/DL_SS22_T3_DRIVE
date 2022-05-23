# Gym-CarRacing starten

## (Windows 10 mit [PyCharm Community Edition](https://www.jetbrains.com/pycharm/download))

### Repository
`git clone https://github.com/andywu0913/OpenAI-GYM-CarRacing-DQN.git`

### Swig
1. [Swig](http://prdownloads.sourceforge.net/swig/swigwin-4.0.2.zip) herunterladen 
2. Swig entpacken (z.B. `C:/`)
3. Wurzelpfad von Swig in der Umgebungsvariable von Windows aufnehmen

### Limitierung der Pfadlänge von Windows aufheben 
Windows 10 Pro über Gruppenrichtlinien siehe [pc-magazin](https://www.pc-magazin.de/ratgeber/windows-maximale-pfadlaenge-andern-anleitung-3197751.html)

#### Windows10 Home
Falls unter Windows mit `Win + R` der Befehl `gpedit.msc` nicht gefunden wird, muss der Editor für Gruppenrichtlinien manuell installiert werden:
Tutorial unter #3 auf [minitool](https://www.minitool.com/news/group-policy-editor-gpedit-msc-missing.html)

### Mujoco
1. Mujoco 1.5 (150) von der [Website](https://www.roboti.us/download/mjpro150_win64.zip) herunterladen
2. Mujoco im folgenden Verzeichnis entpacken `C:\Users\Benutzername\.mujoco`, sodass der `bin` Ordner als Unterordner im folgenden Verzeichnis verfügbar ist `C:\Users\Benutzername\.mujoco\mjpro150`
3. Activation Key von [Mujoco](https://www.roboti.us/license.html) herunterladen  und dann auf Activation Key Link klicken
4. Activation Key im Verzeichnis `C:\Users\Benutzername\.mujoco` ablegen, damit der Ordner `mjpro150` und die Textdatei auf einer Ebene liegen

### Gym (über [PyCharm](https://www.jetbrains.com/pycharm/download) installieren)
Über Python Console in Pycharm folgende Anweisungen eingeben
1. `import pip`
2. `pip.main(['install','--upgrade','setuptools'])`
3. `pip.main(['install','numpy','gym[box2d]'])`

### Alle Abhängigkeiten installieren
Console als Admin ausführen 
`pip install -r requirements.txt`