#!/usr/bin/env node

/*
 * generateLatex.js
 *
 * Usage:
 *   node generateLatex.js [inputFile] [outputFile]
 *
 * Example:
 *   node generateLatex.js combined_source_files combined_source_files.tex
 *
 * Then run your LaTeX compiler (e.g., pdflatex) on the generated .tex file.
 */

const fs = require("fs")
const path = require("path")

// Regex to detect Java/TypeScript method or function signatures
// Adjust if needed (e.g., for different naming conventions).
const METHOD_START_PATTERN = new RegExp(
    "^\\s*(public|private|protected)?\\s*(static)?\\s*\\S+\\s+\\w+\\s*\\([^)]*\\)\\s*\\{|" +
    "^\\s*function\\s+\\w+\\s*\\([^)]*\\)\\s*\\{"
)

function main()
{
    const inputFile = process.argv[2] || "combined_source_files"
    const outputFile = process.argv[3] || "combined_source_files.tex"

    // Read source code
    const code = fs.readFileSync(inputFile, "utf8")

    // Detect file extension for language
    const ext = path.extname(inputFile).toLowerCase()
    let language
    if (ext === ".ts")
    {
        language = "typescript"
    } else
    {
        language = "Java"
    }

    // Split code into lines
    const lines = code.split(/\r?\n/)

    // Collect chunks (each chunk represents a method or block)
    const chunks = []
    let currentChunk = []
    let inMethodBlock = false
    let braceDepth = 0

    for (const line of lines)
    {
        // Check if we're at the start of a new method
        if (METHOD_START_PATTERN.test(line))
        {
            // If we were accumulating a chunk, push it
            if (currentChunk.length > 0)
            {
                chunks.push(currentChunk.join("\n"))
                currentChunk = []
            }
            inMethodBlock = true
            braceDepth = 0
        }

        // Add the current line to the current chunk
        currentChunk.push(line)

        // If we’re inside a method, track braces to know when it ends
        if (inMethodBlock)
        {
            const openBraces = (line.match(/{/g) || []).length
            const closeBraces = (line.match(/}/g) || []).length
            braceDepth += (openBraces - closeBraces)

            // Once brace depth is zero or below, the method is complete
            if (braceDepth <= 0)
            {
                inMethodBlock = false
                chunks.push(currentChunk.join("\n"))
                currentChunk = []
            }
        }
    }

    // Handle any leftover lines outside methods
    if (currentChunk.length > 0)
    {
        chunks.push(currentChunk.join("\n"))
    }

    // Build LaTeX body: each chunk in its own listing, preceded by \Needspace
    let bodyContent = ""
    for (const chunk of chunks)
    {
        const lineCount = chunk.split(/\r?\n/).length
        // \Needspace{n} tries to keep n lines together
        // Add a little extra buffer to be safe
        bodyContent += `\\Needspace{${lineCount + 2}\\baselineskip}\n`
        bodyContent += `\\begin{lstlisting}[language=${language}]\n`
        bodyContent += chunk
        bodyContent += `\n\\end{lstlisting}\n\n`
    }

    // Final LaTeX document
    const latexContent = `
\\documentclass[12pt]{article}
\\usepackage[margin=1in,paperwidth=8.5in,paperheight=11in]{geometry}
\\usepackage[T1]{fontenc}
\\usepackage[scaled=0.85]{beramono}  % Clean monospaced font
\\renewcommand{\\familydefault}{\\ttdefault}
\\usepackage{xcolor}
\\usepackage{listings}
\\usepackage{needspace}  % For \\Needspace to avoid splitting chunks

% No page numbers:
\\pagestyle{empty}

% Code listing settings (no line numbers):
\\lstset{
  basicstyle=\\footnotesize\\ttfamily,
  breaklines=true,
  showstringspaces=false,
  tabsize=2
}

\\begin{document}

${bodyContent}

\\end{document}
`

    // Write LaTeX to file
    fs.writeFileSync(outputFile, latexContent, "utf8")
    console.log(`LaTeX file generated: ${outputFile}`)
}

main()