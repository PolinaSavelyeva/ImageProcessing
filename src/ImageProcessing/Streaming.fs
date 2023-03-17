module Streaming

open System
open CPUImageProcessing

type message =
    | Image of MyImage
    | Path of string
    | EOS of AsyncReplyChannel<unit>

let imageSaver outputDirectory =

    let initial (inbox: MailboxProcessor<message>) =

        let rec loop () =
            async {
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn $"Image saver across the %A{channel.GetType} is finished!"
                    channel.Reply()
                | Image image ->
                    printfn $"Save: %A{image.Name}"
                    saveMyImage image (generatePath outputDirectory image.Name)
                    return! loop ()
                | _ -> failwith "Wrong message was received in imageSaver. Expected MyImage or AsyncReplyChannel<unit> message type. "
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
                    printfn $"Image processor across the %A{channel.GetType} is ready to finish!"
                    receiver.PostAndReply EOS
                    printfn $"Image processor across the %A{channel.GetType} is finished!"
                    channel.Reply()
                | Image image ->
                    printfn $"Filter: %A{image.Name}"
                    let filtered = imageEditor image
                    receiver.Post(Image filtered)
                    return! loop ()
                | _ -> failwith "Wrong message was received in imageProcessor. Expected MyImage or AsyncReplyChannel<unit> message type. "
            }

        loop ()

    MailboxProcessor.Start initial

let imageFullProcessor imageEditor outputDirectory =

    let initial (inbox: MailboxProcessor<message>) =

        let rec loop () =
            async {
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn $"Image processor and saver is finished!"
                    channel.Reply()
                | Path path ->
                    let image = loadAsMyImage path
                    let filtered = imageEditor image
                    saveMyImage filtered (generatePath outputDirectory image.Name)
                    return! loop ()
                | _ -> failwith "Wrong message was received in imageFullProcessor. Expected string or AsyncReplyChannel<unit> message type. "
            }

        loop ()

    MailboxProcessor.Start initial

// Full uses a single agent to open, process and save
// Partial uses different agents for each transformation and saving
// PartialUsingComposition uses one agent for transformation and one - for save
// None uses naive image processing function

type AgentsSupport =
    | Full
    | Partial
    | PartialUsingComposition
    | No

let processAllFiles inputDirectory outputDirectory imageEditorsList agentsSupport =

    let filesToProcess = listAllImages inputDirectory

    match agentsSupport with
    | Full ->
        let imageEditor = List.fold (>>) id imageEditorsList

        let processorsArray =
            Array.init Environment.ProcessorCount (fun _ -> imageFullProcessor imageEditor outputDirectory)

        for file in filesToProcess do
            (Array.minBy (fun (p: MailboxProcessor<message>) -> p.CurrentQueueLength) processorsArray)
                .Post(Path file)

        for imgProcessor in processorsArray do
            imgProcessor.PostAndReply EOS

    | Partial ->
        let imageProcessor =
            List.foldBack imageProcessor imageEditorsList (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(loadAsMyImage file))

        imageProcessor.PostAndReply EOS
    | PartialUsingComposition ->
        let imageProcessor =
            imageProcessor (List.fold (>>) id imageEditorsList) (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(loadAsMyImage file))

        imageProcessor.PostAndReply EOS
    | No ->
        let imageProcessAndSave path =
            let image = loadAsMyImage path
            let editedImage = image |> List.fold (>>) id imageEditorsList
            generatePath image.Name outputDirectory |> saveMyImage editedImage

        List.map imageProcessAndSave filesToProcess |> ignore
