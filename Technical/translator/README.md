# Translation Script Documentation

## English

### Description
This Python script automates the translation of `.resx` resource files for a project. It takes the base resource file (`AppRes.resx`), detects other language-specific `.resx` files in the same directory, and translates missing or outdated entries using Google Translate. It provides a menu-driven interface for user interaction or can be run with command-line arguments. Users can specify languages to exclude from translation.

### How it Works
1. The script checks for `AppRes.resx` in the given directory.
2. It iterates over all `.resx` files, excluding specified languages.
3. It loads existing translations if available; otherwise, it creates a new `.resx` file.
4. It translates missing or outdated entries using Google Translate.
5. The translated text is inserted into the respective `.resx` files.
6. The script allows users to run it in interactive mode or via command-line arguments.

### Compilation & Execution

#### Linux
1. Ensure Python 3 is installed:
   ```sh
   sudo apt install python3
   ```
2. Install dependencies:
   ```sh
   pip install deep-translator
   ```
3. Run the script:
   ```sh
   python3 main.py --new-only --resx-directory /path/to/resx --exclude-languages "fr,de"
   ```
4. Create an executable:
   ```sh
   pyinstaller --onefile --console --hidden-import=deep_translator.main main.py
   ```

#### Windows
1. Ensure Python 3 is installed (download from [Python website](https://www.python.org/downloads/)).
2. Install dependencies:
   ```cmd
   pip install deep-translator
   ```
3. Run the script:
   ```cmd
   python main.py --new-only --resx-directory C:\path\to\resx --exclude-languages "fr,de"
   ```
4. Create an executable:
   ```cmd
   pyinstaller --onefile --console --hidden-import=deep_translator.main main.py
   ```

### Dependencies
- `deep-translator` (install using `pip install deep-translator`)

### Command-Line Arguments
- `--resx-directory <directory>`: Specifies the path to the `.resx` files directory.
- `--exclude-languages <languages>`: Comma-separated list of languages to exclude (e.g., `fr,de`).
- `--force`: Translates all entries regardless of existing values.
  - **Example:**
    ```sh
    python main.py --force --resx-directory /path/to/resx --exclude-languages "es,it"
    ```
- `--new-only`: Translates only missing or new entries.
  - **Example:**
    ```sh
    python main.py --new-only --resx-directory /path/to/resx --exclude-languages "fr,de"
    ```

---

## Magyar

### Leírás
Ez a Python szkript automatikusan lefordítja a `.resx` erőforrásfájlokat egy projekt számára. Az alap `AppRes.resx` fájl alapján létrehozza vagy frissíti a többi nyelvi `.resx` fájlt a könyvtárban, és a hiányzó vagy elavult bejegyzéseket a Google Fordító segítségével lefordítja. Használható interaktív menüből vagy parancssori argumentumokkal. A felhasználók megadhatják a kizárni kívánt nyelveket.

### Működés
1. A szkript ellenőrzi az `AppRes.resx` fájl létezését.
2. Bejárja az összes `.resx` fájlt, kivéve a megadott nyelveket.
3. Betölti a meglévő fordításokat, vagy új `.resx` fájlt hoz létre.
4. A hiányzó vagy elavult bejegyzéseket a Google Fordítóval lefordítja.
5. A lefordított szövegeket beilleszti a megfelelő `.resx` fájlokba.
6. A szkript interaktív menüből vagy parancssorból is futtatható.

### Fordítás és futtatás

#### Linux
1. Telepítsd a Python 3-at:
   ```sh
   sudo apt install python3
   ```
2. Telepítsd a függőségeket:
   ```sh
   pip install deep-translator
   ```
3. Futtasd a szkriptet:
   ```sh
   python3 main.py --new-only --resx-directory /path/to/resx --exclude-languages "fr,de"
   ```
4. Hozz létre egy futtatható fájlt:
   ```sh
   pyinstaller --onefile --console --hidden-import=deep_translator.main main.py
   ```

#### Windows
1. Telepítsd a Python 3-at ([Python letöltés](https://www.python.org/downloads/)).
2. Telepítsd a függőségeket:
   ```cmd
   pip install deep-translator
   ```
3. Futtasd a szkriptet:
   ```cmd
   python main.py --new-only --resx-directory C:\path\to\resx --exclude-languages "fr,de"
   ```
4. Hozz létre egy futtatható fájlt:
   ```cmd
   pyinstaller --onefile --console --hidden-import=deep_translator.main main.py
   ```

### Függőségek
- `deep-translator` (telepíthető `pip install deep-translator` paranccsal)

### Parancssori argumentumok
- `--resx-directory <könyvtár>`: Az `.resx` fájlokat tartalmazó könyvtár elérési útja.
- `--exclude-languages <nyelvek>`: Kizárt nyelvek vesszővel elválasztott listája (pl. `fr,de`).
- `--force`: Az összes bejegyzést lefordítja, még ha már léteznek is.
  - **Példa:**
    ```sh
    python main.py --force --resx-directory /path/to/resx --exclude-languages "es,it"
    ```
- `--new-only`: Csak az új vagy hiányzó bejegyzéseket fordítja le.
  - **Példa:**
    ```sh
    python main.py --new-only --resx-directory /path/to/resx --exclude-languages "fr,de"
    ```

