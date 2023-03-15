module Streaming

open CPUImageProcessing

type message =
    | Image of MyImage
    | EOS of AsyncReplyChannel<unit>

let generatePath outputDirectory (imageName: string) =
    System.IO.Path.Combine(outputDirectory, imageName)

let imageSaver outputDirectory =

    let initial (inbox: MailboxProcessor<message>) =
        let rec loop () =
            async {
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn "Image saver is finished!"
                    channel.Reply()
                | Image image ->
                    printfn $"Save: %A{image.Name}"
                    saveMyImage image (generatePath outputDirectory image.Name)
                    return! loop ()
            }

        loop ()

    MailboxProcessor.Start initial

let imageProcessor imageEditor (receiver: MailboxProcessor<_>) =

    let initial (inbox: MailboxProcessor<message>) =

        let rec loop () =
            async {
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn "Image processor is ready to finish!"
                    receiver.PostAndReply EOS
                    printfn "Image processor is finished!"
                    channel.Reply()
                | Image image ->
                    printfn $"Filter: %A{image.Name}"
                    let filtered = imageEditor image
                    receiver.Post(Image filtered)
                    return! loop ()
            }

        loop ()

    MailboxProcessor.Start initial

let processAllFilesA inputDirectory outputDirectory imageEditorsList (agentsSupport: bool) =

    let filesToProcess = listAllImages inputDirectory

    if agentsSupport then
        let imageProcessor =
            List.foldBack imageProcessor imageEditorsList (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(loadAsMyImage file))

        imageProcessor.PostAndReply EOS
    else
        let imageProcessAndSave path =
            let image = loadAsMyImage path
            let editedImage = image |> List.fold (>>) id imageEditorsList
            generatePath image.Name outputDirectory |> saveMyImage editedImage

        List.map imageProcessAndSave filesToProcess |> ignore

let processAllFilesB inputDirectory outputDirectory imageEditorsList (agentsSupport: bool) =

    let filesToProcess = listAllImages inputDirectory

    if agentsSupport then
        let imageProcessor =
            imageProcessor (List.fold (>>) id imageEditorsList) (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(loadAsMyImage file))

        imageProcessor.PostAndReply EOS
    else
        let imageProcessAndSave path =
            let image = loadAsMyImage path
            let editedImage = image |> List.fold (>>) id imageEditorsList
            generatePath image.Name outputDirectory |> saveMyImage editedImage

        List.map imageProcessAndSave filesToProcess |> ignore
