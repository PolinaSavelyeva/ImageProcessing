module Streaming

open CPUImageProcessing

type message =
    | Image of byte[,] * string
    | EOS of AsyncReplyChannel<unit>

let imageSaver outputDirectory =

    let generatePath (imageName: string) =
        System.IO.Path.Combine(outputDirectory, imageName)

    let initial (inbox : MailboxProcessor<message>) =
            let rec loop () =
                async {
                    let! message = inbox.Receive()

                    match message with
                    | EOS channel ->
                        printfn "Image saver is finished!"
                        channel.Reply()
                    | Image (data, name) ->
                        printfn $"Save: %A{name}"
                        save2DArrayAsImage data (generatePath name)
                        return! loop ()
                }

            loop ()

    MailboxProcessor.Start initial

let imageProcessor imageEditor ( receiver : MailboxProcessor<_>) =

    let initial (inbox : MailboxProcessor<message>) =

        let rec loop () =
            async {
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn "Image processor is ready to finish!"
                    receiver.PostAndReply EOS
                    printfn "Image processor is finished!"
                    channel.Reply()
                | Image (data, name) ->
                    printfn $"Filter: %A{name}"
                    let filtered = imageEditor data
                    receiver.Post(Image (filtered, name))
                    return! loop ()
            }

        loop ()
    MailboxProcessor.Start initial

let processAllFilesA inputDirectory outputDirectory imageEditorsList  =

    (*let imageProcessors outputDirectory =
        imageEditorsList
        |> List.map (fun x ->
            let imageSaver = imageSaver outputDirectory
            imageProcessor x imageSaver)
        |> Array.ofList

        let rec inner list =
            match list with
            | [] ->
                failwith "No transformations"
            | [hd] -> imageProcessor hd (imageSaver outputDirectory), [imageProcessor hd (imageSaver outputDirectory)]
            | hd1 :: hd2 :: tl ->
                        let x = inner (hd2 :: tl)
                        imageProcessor hd1 (fst x), imageProcessor hd1 (fst x) :: snd x

        List.toArray (snd <| inner imageEditorsList)*)

    (*let imageProcessor =

        let rec inner list =
            match list with
            | [] ->
                failwith "No transformations"
            | [hd] -> imageProcessor hd (imageSaver outputDirectory)
            | hd1 :: hd2 :: tl ->
                        imageProcessor hd1 (inner (hd2 :: tl))
        inner imageEditorsList*)
    let imageProcessor = List.fold (fun acc x -> imageProcessor x acc) (imageSaver outputDirectory) imageEditorsList

    let filesToProcess = listAllImages inputDirectory

    for file in filesToProcess do
            imageProcessor.Post(Image(loadAs2DArray file, System.IO.Path.GetFileName file))

    imageProcessor.PostAndReply EOS
//imgSaver.PostAndReply EOS

let processAllFilesB inputDirectory outputDirectory imageEditorsList  =

    let imageProcessor = imageProcessor (List.fold (>>) id imageEditorsList) (imageSaver outputDirectory)
    
    let filesToProcess = listAllImages inputDirectory

    for file in filesToProcess do
            imageProcessor.Post(Image(loadAs2DArray file, System.IO.Path.GetFileName file))

    imageProcessor.PostAndReply EOS
