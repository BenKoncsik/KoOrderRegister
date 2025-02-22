package main

import (
	"bufio"
	"fmt"
	"io"
	"os"
	"path/filepath"
	"strings"
	"time"

	"github.com/manifoldco/promptui"
)

const chunkSize = 50 * 1024 * 1024 // 50 MB

func main() {
	for {
		menu := []string{"Split .kncsk file", "Reassemble .kncsk file", "Exit"}
		prompt := promptui.Select{
			Label: "Select an action",
			Items: menu,
		}

		_, result, err := prompt.Run()
		if err != nil {
			fmt.Println("Prompt failed:", err)
			continue
		}

		switch result {
		case "Split .kncsk file":
			splitFile()
		case "Reassemble .kncsk file":
			reassembleFile()
		case "Exit":
			fmt.Println("Exiting...")
			return
		}
	}
}

func splitFile() {
	files, err := filepath.Glob("*.kncsk")
	if err != nil || len(files) == 0 {
		fmt.Println("No .kncsk files found!")
		return
	}

	prompt := promptui.Select{
		Label: "Select a .kncsk file to split",
		Items: files,
	}

	_, selectedFile, err := prompt.Run()
	if err != nil {
		fmt.Println("Selection failed:", err)
		return
	}

	file, err := os.Open(selectedFile)
	if err != nil {
		fmt.Println("Error opening file:", err)
		return
	}
	defer file.Close()

	fileInfo, _ := file.Stat()
	dirName := strings.TrimSuffix(selectedFile, ".kncsk")
	os.MkdirAll(dirName, 0755)

	buffer := make([]byte, chunkSize)
	reader := bufio.NewReader(file)
	index := 0
	totalSize := fileInfo.Size()
	processed := int64(0)

	for {
		bytesRead, err := reader.Read(buffer)
		if bytesRead == 0 {
			break
		}
		chunkFileName := fmt.Sprintf("%s/part_%03d.pkncsk", dirName, index)
		chunkFile, err := os.Create(chunkFileName)
		if err != nil {
			fmt.Println("Error creating chunk file:", err)
			return
		}
		chunkFile.Write(buffer[:bytesRead])
		chunkFile.Close()
		index++
		processed += int64(bytesRead)
		fmt.Printf("Progress: %.2f%%\r", float64(processed)/float64(totalSize)*100)
		time.Sleep(50 * time.Millisecond)

		if err == io.EOF {
			break
		}
	}

	fmt.Println("\nFile split successfully!")
}

func reassembleFile() {
	dirs, err := os.ReadDir(".")
	if err != nil {
		fmt.Println("Error reading directories:", err)
		return
	}

	var validDirs []string
	for _, dir := range dirs {
		if dir.IsDir() {
			validDirs = append(validDirs, dir.Name())
		}
	}

	if len(validDirs) == 0 {
		fmt.Println("No directories found!")
		return
	}

	prompt := promptui.Select{
		Label: "Select a folder to reassemble",
		Items: validDirs,
	}

	_, selectedDir, err := prompt.Run()
	if err != nil {
		fmt.Println("Selection failed:", err)
		return
	}

	outputFile, err := os.Create(selectedDir + ".kncsk")
	if err != nil {
		fmt.Println("Error creating output file:", err)
		return
	}
	defer outputFile.Close()

	files, err := filepath.Glob(fmt.Sprintf("%s/part_*.pkncsk", selectedDir))
	if err != nil || len(files) == 0 {
		fmt.Println("No chunk files found!")
		return
	}

	totalParts := len(files)

	for i := 0; i < totalParts; i++ {
		chunkFileName := fmt.Sprintf("%s/part_%03d.pkncsk", selectedDir, i)
		chunkFile, err := os.Open(chunkFileName)
		if err != nil {
			fmt.Println("Error opening chunk file:", err)
			return
		}
		io.Copy(outputFile, chunkFile)
		chunkFile.Close()
		fmt.Printf("Progress: %.2f%%\r", float64(i+1)/float64(totalParts)*100)
		time.Sleep(50 * time.Millisecond)
	}

	fmt.Println("\nFile reassembled successfully!")
}
