import os
import sys
import xml.etree.ElementTree as ET
from deep_translator import GoogleTranslator

def translate_text(text, target_lang):
    translator = GoogleTranslator(source='auto', target=target_lang)
    return translator.translate(text)

def process_resx_files(directory, force_translate):
    original_file = os.path.join(directory, "AppRes.resx")
    if not os.path.exists(original_file):
        print("Original resx file not found!")
        return

    tree = ET.parse(original_file)
    root = tree.getroot()
    original_entries = {entry.attrib['name']: entry for entry in root.findall('data')}

    for file_name in os.listdir(directory):
        if file_name.startswith("AppRes.") and file_name.endswith(".resx") and file_name != "AppRes.resx":
            lang_code = file_name[7:-5]  # Extracting language code from filename
            if lang_code == "hu-HU":
                continue

            file_path = os.path.join(directory, file_name)
            if os.path.exists(file_path):
                resx_tree = ET.parse(file_path)
                resx_root = resx_tree.getroot()
            else:
                resx_root = ET.Element("root")
                for element in root.findall("resheader"):
                    resx_root.append(element)
                for schema in root.findall("{http://www.w3.org/2001/XMLSchema}schema"):
                    resx_root.append(schema)

            existing_entries = {entry.attrib['name']: entry for entry in resx_root.findall('data')}

            for name, orig_entry in original_entries.items():
                orig_value_elem = orig_entry.find('value')
                orig_value = orig_value_elem.text if orig_value_elem is not None else ""
                
                if force_translate or name not in existing_entries:
                    translated_value = translate_text(orig_value, lang_code[:2])
                    print(f"Translate ({lang_code}): {orig_value} --> {translated_value}")
                    new_entry = ET.Element("data", name=name)
                    if 'xml:space' in orig_entry.attrib:
                        new_entry.set("xml:space", orig_entry.attrib['xml:space'])
                    value_elem = ET.SubElement(new_entry, "value")
                    value_elem.text = translated_value
                    
                    comment_elem = orig_entry.find("comment")
                    if comment_elem is not None:
                        new_comment = ET.SubElement(new_entry, "comment")
                        new_comment.text = comment_elem.text
                    
                    resx_root.append(new_entry)

            resx_tree = ET.ElementTree(resx_root)
            resx_tree.write(file_path, encoding="utf-8", xml_declaration=True)
            print(f"Translate ({lang_code}): Done!")

    print("\nTranslation process completed!")

def main():
    directory = "../../KoOrderRegister/Localization"
    if len(sys.argv) > 1:
        if sys.argv[1] == "--force":
            process_resx_files(directory, True)
            return
        elif sys.argv[1] == "--new-only":
            process_resx_files(directory, False)
            return
    
    while True:
        print("Menu:")
        print("1. Start translation (only new entries)")
        print("2. Force re-translate all entries")
        print("3. Exit")
        choice = input("Choose an option: ")
        
        if choice == "1":
            process_resx_files(directory, False)
        elif choice == "2":
            process_resx_files(directory, True)
        elif choice == "3":
            print("Exiting...")
            break
        else:
            print("Invalid option, please choose again.")

if __name__ == "__main__":
    main()
