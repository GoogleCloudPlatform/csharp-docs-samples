﻿namespace F_.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http

type HomeController () =
    inherit Controller()

    member this.Index () =
        this.View()

    member this.About () =
        this.ViewData.["Message"] <- "Your application description page."
        this.View()

    member this.Contact () =
        this.ViewData.["Message"] <- "Your contact page."
        this.View()

    member this.Error () =
        this.View();

    member this.Naughty () =
        this.View(new List<IFormFile>());

    [<HttpPost>]
    member this.Naughty (files:List<IFormFile>) =
        this.View(files);
