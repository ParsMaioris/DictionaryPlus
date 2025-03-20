"use strict"

const fs = require("fs").promises
const path = require("path")

/**
 * Recursively collects all .cs and .csproj file paths from the specified directory,
 * skipping any directories named "bin" or "obj".
 *
 * @param {string} dir - The directory to search.
 * @returns {Promise<string[]>} - A list of matching file paths.
 */
async function getSourceFiles(dir)
{
    let files = []
    const entries = await fs.readdir(dir, {withFileTypes: true})
    for (const entry of entries)
    {
        const fullPath = path.join(dir, entry.name)
        if (entry.isDirectory())
        {
            if (entry.name === "bin" || entry.name === "obj") continue
            files = files.concat(await getSourceFiles(fullPath))
        }
        else if (
            entry.isFile() &&
            (entry.name.endsWith(".cs") || entry.name.endsWith(".csproj"))
        )
        {
            files.push(fullPath)
        }
    }
    return files
}

/**
 * Removes the Byte Order Mark (BOM) from text if present.
 *
 * @param {string} text - The text to clean.
 * @returns {string} - The cleaned text.
 */
function removeBom(text)
{
    return text.charCodeAt(0) === 0xFEFF ? text.substring(1) : text
}

/**
 * Combines all source files' content into a single file.
 *
 * @param {string} startDir - The root directory to scan.
 * @param {string} outputFile - The file where the combined output will be saved.
 */
async function combineSourceFiles(startDir, outputFile)
{
    const sourceFiles = await getSourceFiles(startDir)
    let output = ""

    for (const file of sourceFiles)
    {
        let content = await fs.readFile(file, "utf8")
        content = removeBom(content)
        output += `\n=== File: ${file} ===\n${content}\n`
    }

    await fs.writeFile(outputFile, output, "utf8")
}

async function main()
{
    const startDir = process.argv[2] || ".."
    const outputFile = process.argv[3] || "combined_source_files"

    try
    {
        await combineSourceFiles(startDir, outputFile)
        console.log(`Combined source files saved to ${outputFile}`)
    }
    catch (error)
    {
        console.error("Error:", error)
        process.exit(1)
    }
}

main()