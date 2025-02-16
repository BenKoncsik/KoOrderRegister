package main

import (
	"bufio"
	"encoding/json"
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"net/url"
	"os"
	"path/filepath"
	"strings"
)

type Data struct {
	XMLName xml.Name `xml:"root"`
	Entries []Entry  `xml:"data"`
}

type Entry struct {
	Name          string `xml:"name,attr"`
	SpacePreserve string `xml:"xml:space,attr,omitempty"`
	Value         string `xml:"value"`
}

type TranslationResponse struct {
	ResponseData struct {
		TranslatedText string `json:"translatedText"`
	} `json:"responseData"`
}

var excludedLanguages = map[string]bool{
	"hu-HU": true,
}

func translate(text, targetLang string) string {
	apiURL := "https://api.mymemory.translated.net/get"
	params := url.Values{}
	params.Add("q", text)
	params.Add("langpair", "en|"+targetLang)
	resp, err := http.Get(apiURL + "?" + params.Encode())
	if err != nil {
		log.Printf("Translation error: %v", err)
		return text
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		log.Printf("Error reading translation response: %v", err)
		return text
	}

	var result TranslationResponse
	if err := json.Unmarshal(body, &result); err != nil {
		log.Printf("Error parsing translation response: %v", err)
		return text
	}

	return result.ResponseData.TranslatedText
}

func clearLine() {
	fmt.Print("\r\033[K")
}

func processResxFiles(directory string, forceTranslate bool) {
	originalPath := filepath.Join(directory, "AppRes.resx")
	originalData, err := ioutil.ReadFile(originalPath)
	if err != nil {
		log.Fatalf("Failed to read original resx file: %v", err)
	}

	var originalEntries Data
	err = xml.Unmarshal(originalData, &originalEntries)
	if err != nil {
		log.Fatalf("Failed to parse original resx file: %v", err)
	}

	files, err := ioutil.ReadDir(directory)
	if err != nil {
		log.Fatalf("Failed to read directory: %v", err)
	}

	fmt.Println("Starting translation process...")
	for _, file := range files {
		if strings.HasPrefix(file.Name(), "AppRes.") && strings.HasSuffix(file.Name(), ".resx") && file.Name() != "AppRes.resx" {
			langCode := strings.TrimSuffix(strings.TrimPrefix(file.Name(), "AppRes."), ".resx")
			if excludedLanguages[langCode] {
				continue
			}

			filePath := filepath.Join(directory, file.Name())
			resxData, err := ioutil.ReadFile(filePath)
			if err != nil {
				log.Printf("Failed to read resx file %s: %v", file.Name(), err)
				resxData = []byte("<root></root>")
			}

			var resxEntries Data
			xml.Unmarshal(resxData, &resxEntries)

			existingEntries := make(map[string]Entry)
			for _, entry := range resxEntries.Entries {
				existingEntries[entry.Name] = entry
			}

			for _, entry := range originalEntries.Entries {
				if forceTranslate || existingEntries[entry.Name].Value == "" {
					translatedValue := translate(entry.Value, langCode[:2])
					clearLine()
					fmt.Printf("Translate (%s): %s --> %s", langCode, entry.Value, translatedValue)
					newEntry := Entry{Name: entry.Name, SpacePreserve: "preserve", Value: translatedValue}
					resxEntries.Entries = append(resxEntries.Entries, newEntry)
				}
			}

			outputData, err := xml.MarshalIndent(resxEntries, "", "  ")
			if err != nil {
				log.Printf("Failed to marshal updated resx file for %s: %v", langCode, err)
				continue
			}

			err = ioutil.WriteFile(filePath, outputData, 0644)
			if err != nil {
				log.Printf("Failed to write updated resx file for %s: %v", langCode, err)
			}
			clearLine()
			fmt.Printf("Translate (%s): Done!\n", langCode)
		}
	}
	fmt.Println("\nTranslation process completed!")
}

func main() {
	scanner := bufio.NewScanner(os.Stdin)
	for {
		fmt.Println("Menu:")
		fmt.Println("1. Start translation (only new entries)")
		fmt.Println("2. Force re-translate all entries")
		fmt.Println("3. Exit")
		fmt.Print("Choose an option: ")
		scanner.Scan()
		choice := scanner.Text()

		switch choice {
		case "1":
			localizationDir := "../../KoOrderRegister/Localization"
			processResxFiles(localizationDir, false)
		case "2":
			localizationDir := "../../KoOrderRegister/Localization"
			processResxFiles(localizationDir, true)
		case "3":
			fmt.Println("Exiting...")
			return
		default:
			fmt.Println("Invalid option, please choose again.")
		}
	}
}
