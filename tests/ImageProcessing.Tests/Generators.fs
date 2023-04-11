module Generators

open FsCheck

let lengthGen: Gen<int> = Gen.choose (2, 100)

let dataGen length1 length2 : Gen<byte[]> =
    Gen.arrayOfLength (length1 * length2) (Gen.elements [ 0uy .. 127uy ])

let myImageGen: Gen<MyImage.MyImage> =
    gen {
        let! length1 = lengthGen
        let! length2 = lengthGen
        let! data = dataGen length1 length2
        return! Gen.elements [ MyImage.MyImage(data, length1, length2, "MyImage") ]
    }

type MyGenerators =
    static member MyImage() = Arb.fromGen myImageGen
