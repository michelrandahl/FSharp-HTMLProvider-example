#r "../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "System.Xml.Linq.dll"

open FSharp.Data

let base_url = "http://fsharp.github.io"

type HTML = HtmlProvider<"http://fsharp.github.io/FSharp.Data/index.html">

let html = HTML.GetSample()

let type_provider_articles_titles =
    html.Lists.Html.CssSelect "li"
    |> Seq.skipWhile (fun el -> el.InnerText() <> "Documentation")
    |> Seq.skip 3
    |> Seq.takeWhile (fun el -> Seq.length(el.CssSelect "a") > 0)
    |> Seq.map (fun el -> el.CssSelect "a" |> Seq.head)
    |> Seq.map (fun el -> el.Attribute "href")
    |> Seq.map (fun el -> el.Value())
    |> Seq.map ((+) base_url)
    |> Seq.map (HTML.AsyncLoad) // using the same type because all these pages look the same, if they did not look the same then we would need another type to represent them
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Seq.map (fun site -> site.Html.CssSelect "h1" |> Seq.head)
    |> Seq.map (fun el -> el.InnerText())
    |> List.ofSeq
type_provider_articles_titles
|> printfn "%A"
