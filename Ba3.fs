namespace Ba3

open Bio.Parse
open Bio.Type
open Bio.Utils
open FParsec

module Ba3a =
    let compositionK k =
        kmers k >> Set.ofSeq
    let compositionKshow k =
        compositionK k >> Set.map ds2str >> String.concat "\n"
    let pcomp = int_ws .>>. pDNAD

    let ba3a_main pathin pathout =
        let sin = System.IO.File.ReadAllText(pathin)
        match run pcomp sin with
        | Success((k, dna), _, _) ->
            let res = compositionKshow k dna
            System.IO.File.WriteAllText(pathout, res)
            printfn "Answer written to %s" pathout
        | Failure _ -> failwith "Invalid parse"

module Ba3b =
    let ppath = strandsD

    let recon xss =
        let fst = xss |> List.head
        let rst = xss |> List.tail |> List.map List.last
        fst @ rst

    let Ba3b_main () = readWrite "data/ch3/path.txt" ppath (recon >> dl2str)

module Ba3c =
    let pref = init
    let suff = List.tail
    let matchIxs xs ys =
        seq {for i, x in List.indexed xs do
                for j, y in List.indexed ys
                    do if y = x then yield i, j }
    let ff f g xs = (List.map f xs, List.map g xs)
    let mkAlst dnas =
        let prefs, suffs = (ff pref suff) dnas
        // let prefs, suffs = (pref &&& suff) dnas
        let ixs = matchIxs suffs prefs
        let dnaa = Array.ofList dnas
        let alst = [ for i, j in ixs -> dnaa.[i], dnaa.[j] ]
        alst

    let show_alst alst =
        String.concat "\n"
            [ for x, y in alst
                -> sprintf "%s -> %s" (dl2str x) (dl2str y) ]

    let Ba3c_main () = readWrite "data/ch3/rosalind_ba3c.txt"
                        strandsD (mkAlst >> show_alst)

module Ba3d =
    let pdbruijn = int_ws .>>. pDNAD
    let presuf x = (init x, List.tail x)

    let grpDBG (k, dna) =
        kmerl k dna |> List.ofSeq |> List.map presuf
        |> List.sort |> List.groupBy fst

    let dedupeTup x = (fst &&& (snd >> List.map snd)) x
    let showTup k = k |> (dl2str *** (List.map dl2str >> String.concat ","))
                    |> (fun (x, y) -> String.concat " -> " [|x; y|])
    let showTups ks = List.map showTup ks |> String.concat "\n"
    let solveDbg vals = grpDBG vals |> List.map dedupeTup |> showTups


