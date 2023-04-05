module Streaming

open MyImage

type imageMessage =
    | Image of MyImage
    | EOS of AsyncReplyChannel<unit>

let generatePath outputDirectory (imageName: string) =
    System.IO.Path.Combine(outputDirectory, imageName)

let imageSaver outputDirectory =

    let initial (inbox: MailboxProcessor<imageMessage>) =

        async {
            while true do
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn $"Image saver across the %A{channel.GetType} is finished!"
                    channel.Reply()

                | Image image ->
                    printfn $"Save: %A{image.Name}"
                    save image (generatePath outputDirectory image.Name)
        }

    MailboxProcessor.Start initial

let imageProcessor imageEditor (receiver: MailboxProcessor<_>) =

    let initial (inbox: MailboxProcessor<imageMessage>) =

        async {
            while true do
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
        }

    MailboxProcessor.Start initial

type pathMessage =
    | Path of string
    | EOS of AsyncReplyChannel<unit>

let imageFullProcessor imageEditor outputDirectory =

    let initial (inbox: MailboxProcessor<pathMessage>) =

        async {
            while true do
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    printfn "Image processor and saver is finished!"
                    channel.Reply()

                | Path path ->
                    let image = load path
                    let filtered = imageEditor image
                    save filtered (generatePath outputDirectory image.Name)
        }

    MailboxProcessor.Start initial
