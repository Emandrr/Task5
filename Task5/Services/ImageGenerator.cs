using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Random;

public class ImageGenerator
{
    private Random random;

    public void ChangeSeed(int seed,int bias)
    {
        random = new WH2006(seed+bias);
    }
    private (string color1, string color2) GenerateRandomDarkColors()
    {
        var color1 = $"rgb({random.Next(0, 150)}, {random.Next(0, 150)}, {random.Next(0, 150)})";
        var color2 = $"rgb({random.Next(0, 150)}, {random.Next(0, 150)}, {random.Next(0, 150)})";
        return (color1, color2);
    }

    private string GenerateRandomColor()
    {
        return $"rgb({random.Next(150, 256)}, {random.Next(150, 256)}, {random.Next(150, 256)})";
    }

    private string CreateRandomLine()
    {
        var strokeWidth = random.Next(1, 4);
        var strokeOpacity = (double)random.Next(1, 5) / 10;
        var randomColor = GenerateRandomColor();

        return $"<line x1='{random.Next(0, 800)}' y1='{random.Next(0, 1600)}' " +
               $"x2='{random.Next(0, 800)}' y2='{random.Next(0, 1600)}' " +
               $"style='stroke:{randomColor};stroke-width:{strokeWidth};stroke-opacity:{strokeOpacity}'/>";
    }

    private string GenerateRandomLines()
    {
        var lineCount = random.Next(5, 15);
        var lineSvg = "";

        for (int i = 0; i < lineCount; i++)
        {
            lineSvg += CreateRandomLine();
        }

        return lineSvg;
    }

    private string GenerateRandomBackgroundSvg(string color1, string color2)
    {
        var lineSvg = GenerateRandomLines();

        return $@"
        <rect width='800' height='1600' fill='{color1}' />
        {lineSvg}";
    }

    private bool IsTextEmpty(string text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    private string[] GetWords(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    private (string firstLine, string secondLine) SplitWords(string[] words)
    {
        int midPoint = (words.Length + 1) / 2;

        string firstLine = string.Join(" ", words.Take(midPoint));
        string secondLine = string.Join(" ", words.Skip(midPoint));

        return (firstLine, secondLine);
    }

    public string PrepareString(string text)
    {
        if (IsTextEmpty(text)) return "";

        string[] words = GetWords(text);
        if (words.Length == 0) return "";
        if (words.Length == 1) return words[0];

        var (firstLine, secondLine) = SplitWords(words);
        return $"{firstLine}\n{secondLine}";
    }

    private int CalculateLineHeight(int fontSize)
    {
        return (int)(fontSize * 1.2);
    }

    private int CalculateStartY(int y, int fontSize, int lineCount)
    {
        int lineHeight = CalculateLineHeight(fontSize);
        int totalHeight = lineCount * lineHeight;
        return y - (totalHeight / 2) + (lineHeight / 2);
    }

    private string CreateSingleTextLine(string text, int x, int y, int fontSize, string fontFamily, string fill, string filter)
    {
        return $@"
            <text x='{x}' y='{y}' font-family='{fontFamily}' font-size='{fontSize}' font-weight='bold' 
                  text-anchor='middle' fill='{fill}' filter='{filter}'>
                {System.Security.SecurityElement.Escape(text.Trim())}
            </text>";
    }

    private string CreateMultilineText(string text, int x, int y, int fontSize, string fontFamily = "Arial", string fill = "white", string filter = "url(#shadow)")
    {
        string[] lines = text.Split('\n');
        StringBuilder textElements = new StringBuilder();

        int lineHeight = CalculateLineHeight(fontSize);
        int startY = CalculateStartY(y, fontSize, lines.Length);

        return BuildTextElements(lines, x, startY, lineHeight, fontSize, fontFamily, fill, filter);
    }

    private string BuildTextElements(string[] lines, int x, int startY, int lineHeight, int fontSize, string fontFamily, string fill, string filter)
    {
        StringBuilder textElements = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            int lineY = startY + (i * lineHeight);
            textElements.AppendLine(
                CreateSingleTextLine(lines[i], x, lineY, fontSize, fontFamily, fill, filter)
            );
        }

        return textElements.ToString();
    }

    private string CreateSvgDefinitions()
    {
        return $@"
            <defs>
                <filter id='shadow' x='-20%' y='-20%' width='140%' height='140%'>
                    <feDropShadow dx='2' dy='2' stdDeviation='2' flood-color='rgba(0,0,0,0.8)'/>
                </filter>
            </defs>";
    }

    private string BuildSvgContent(string backgroundSvg, string authorText, string titleText)
    {
        var definitions = CreateSvgDefinitions();

        return $@"
        <svg width='800' height='1600' xmlns='http://www.w3.org/2000/svg'>
            {definitions}
            {backgroundSvg}
            {authorText}
            {titleText}
        </svg>";
    }

    private string ConvertToBase64(string svg)
    {
        var svgBytes = System.Text.Encoding.UTF8.GetBytes(svg);
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(svgBytes)}";
    }

    public string ImageWithCanvas(string author, string title)
    {
        var (color1, color2) = GenerateRandomDarkColors();
        var backgroundSvg = GenerateRandomBackgroundSvg(color1, color2);

        var authorPrepared = PrepareString(author);
        var titlePrepared = PrepareString(title);

        var authorText = CreateMultilineText(authorPrepared, 400, 300, 66);
        var titleText = CreateMultilineText(titlePrepared, 400, 1300, 66);

        var svg = BuildSvgContent(backgroundSvg, authorText, titleText);
        return ConvertToBase64(svg);
    }
}