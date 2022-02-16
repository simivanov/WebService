Imports System
Imports System.Threading

Module Program
    Sub Main(args As String())


        Dim webService As New WebService

        Try
            Dim thread As New Thread(AddressOf webService.StartServer)
            thread.Start()
        Catch ex As Exception
            Console.WriteLine("The web service is not working!")
        End Try


    End Sub
End Module
