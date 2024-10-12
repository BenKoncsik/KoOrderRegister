package main

import (
	"bufio"
	"fmt"
	"os"
	"path/filepath"
	"regexp"
	"strings"
)

func main() {
	fmt.Println("Starting SVG Converter...")
	createDirectories()
	for {
		fmt.Println("SVG Converter Menu:")
		fmt.Println("1. Convert SVG files")
		fmt.Println("0. Exit")
		fmt.Print("Choose an option: ")

		scanner := bufio.NewScanner(os.Stdin)
		scanner.Scan()
		choice := scanner.Text()

		fmt.Printf("User selected option: %s\n", choice)

		switch choice {
		case "1":
			fmt.Println("Starting SVG file conversion...")
			convertSVGFiles()
		case "0":
			fmt.Println("Exiting... Goodbye!")
			return
		default:
			fmt.Println("Invalid choice. Please try again.")
		}
	}
}

func createDirectories() {
	inputDir := "input"
	outputDir := "output"

	fmt.Println("Checking if input and output directories exist...")

	if _, err := os.Stat(inputDir); os.IsNotExist(err) {
		fmt.Printf("Input directory does not exist. Creating directory: %s\n", inputDir)
		os.Mkdir(inputDir, os.ModePerm)
		fmt.Printf("Input directory created: %s\n", inputDir)
	} else {
		fmt.Printf("Input directory already exists: %s\n", inputDir)
	}

	if _, err := os.Stat(outputDir); os.IsNotExist(err) {
		fmt.Printf("Output directory does not exist. Creating directory: %s\n", outputDir)
		os.Mkdir(outputDir, os.ModePerm)
		fmt.Printf("Output directory created: %s\n", outputDir)
	} else {
		fmt.Printf("Output directory already exists: %s\n", outputDir)
	}
}

func convertSVGFiles() {
	inputDir := "input"
	outputDir := "output"

	fmt.Printf("Reading SVG files from directory: %s\n", inputDir)
	svgFiles, err := filepath.Glob(filepath.Join(inputDir, "*.svg"))
	if err != nil {
		fmt.Printf("Error reading SVG files: %v\n", err)
		return
	}

	fmt.Printf("Number of SVG files found: %d\n", len(svgFiles))

	if len(svgFiles) == 0 {
		fmt.Println("No SVG files found in the input directory.")
		return
	}

	for _, svgFile := range svgFiles {
		fmt.Printf("Converting file: %s\n", svgFile)
		convertFile(svgFile, outputDir)
	}
	fmt.Println("Conversion complete!")
}

func convertFile(filePath string, outputDir string) {
	fmt.Printf("Reading file: %s\n", filePath)
	content, err := os.ReadFile(filePath)
	if err != nil {
		fmt.Printf("Error reading file (%s): %v\n", filePath, err)
		return
	}

	fmt.Println("Modifying SVG content for light version...")
	lightContent := modifyFillAttribute(string(content), "#000000")
	fmt.Println("Modifying SVG content for dark version...")
	darkContent := modifyFillAttribute(string(content), "#FFFFFF")

	baseName := strings.TrimSuffix(filepath.Base(filePath), ".svg")
	lightFilePath := filepath.Join(outputDir, baseName+"_light.svg")
	darkFilePath := filepath.Join(outputDir, baseName+"_dark.svg")

	fmt.Printf("Writing light version to: %s\n", lightFilePath)
	os.WriteFile(lightFilePath, []byte(lightContent), os.ModePerm)
	fmt.Printf("Writing dark version to: %s\n", darkFilePath)
	os.WriteFile(darkFilePath, []byte(darkContent), os.ModePerm)

	fmt.Printf("Files created: %s, %s\n", lightFilePath, darkFilePath)
}

func modifyFillAttribute(content string, fillColor string) string {
	fmt.Printf("Modifying fill attribute to: %s\n", fillColor)
	fillRegex := regexp.MustCompile(`fill="#[a-fA-F0-9]{6}"`)
	if fillRegex.MatchString(content) {
		fmt.Println("Existing fill attribute found. Replacing...")
		return fillRegex.ReplaceAllString(content, fmt.Sprintf("fill=\"%s\"", fillColor))
	}
	// If no fill attribute, insert it after the first <svg> tag
	fmt.Println("No existing fill attribute found. Inserting new fill attribute...")
	return strings.Replace(content, "<svg", "<svg fill=\""+fillColor+"\"", 1)
}
