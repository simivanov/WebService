#Region "Imports"

Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.Xml.Serialization
Imports System.IO
Imports System.Web
Imports System.Security.Cryptography
Imports System.Data.SqlClient
Imports CustomLibrary.Data
Imports CustomLibrary.Setup
Imports System.Globalization
Imports CustomLibrary.Nomenclatures
Imports CustomLibrary.Common
Imports CustomLibrary.Operations
Imports System.Xml
Imports CustomLibrary.Resources

#End Region


Public Class WebService

#Region "Declarations"

        ' DataSet обект за прехвърляне на номенклатури
        Private NomenclatureDataset As DataSet
        ' Database обект за връзка към основната база данни
        Friend dbApplication As Database
        ' Database обект за връзка към репликационен сървър с база данни
        Friend dbReplication As Database
        ' Обект за работа с настройките
        Friend thisConfig As Configuration
        Friend user As Users
        Private partnerStruct As TPartner
        Private operation As CustomLibrary.Operations.Sale
        Private stringXML As StringBuilder

#End Region

#Region "Nested Classes"

        Private Class RepeatedOperationException
            Inherits System.Exception

            Public Overrides ReadOnly Property Message() As String
                Get
                    Return "<ROOT><STATUS>SUCCESS</STATUS><OPAERATION_ID></OPAERATION_ID><ACCT></ACCT></ROOT>"
                End Get
            End Property

            Public Sub New()

            End Sub

        End Class

        Private Class OperationObject

            ' <summary> Константа за представяне на номерата на документите</summary>
            Const acctFormat As String = "0000000000"
            'Полета които се сетват от XML
            Private idField As String
            Private fiscalDeviceSNField As String
            Private fmidField As String
            Private unpField As String
            Private companyIdField As String
            Private dateOpenedField As String
            Private dateIssuedField As Date
            Private operTypeField As String
            Private objectIDField As String
            Private partnerIdField As String
            Private userIdField As String
            Private amountField As String
            Private amountVatField As String
            Private pricesWithVatField As String
            Private nullVatField As String
            Private isVatField As String
            Private paymentTypeIdField As String

            Private itemIdField As String
            Private qttyField As String
            Private discountField As String
            Private priceField As String
            Private rowSumField As String
            Private noteField As String

            Private paymentIdField As String
            Private amountPField As String

            Private totalField As String
            Private fmidRField As String
            Private ecridField As String
            Private receiptIdField As String
            Private receiptTypeField As String
            'Допълнителни полета
            Private docDateField As Date = statusBarTag
            Dim acct As Integer
            Private userRealTimeField As String = "GETDATE()"
            Dim sign As Integer = -1
            Private lotField As String = " "
            Private lotIdField As Integer = 1
            Private currencyIdField As Integer = 1
            Private currencyRateField As Double = 1
            Private srcDocIdField As Integer = 0
            Dim transactionNumber As String = " "
            Private paymentTypeField As Integer = 1
            Private currentSignField As Integer = 1
            Private operTypeForCashbookField As Integer = 8
            Private descriptionForCashbookField As String = Dictionary.GetResource("strSaleOp") & " " & Dictionary.GetResource("strNo") & " " & Format(acct, acctFormat)
            Dim vat As Double = 0

            Public Property Id() As String
                Get
                    Return Me.idField
                End Get
                Set(ByVal value As String)
                    Me.idField = value
                End Set
            End Property

            Public Property FiscalDeviceSN() As String
                Get
                    Return Me.fiscalDeviceSNField
                End Get
                Set(ByVal value As String)
                    Me.fiscalDeviceSNField = value
                End Set
            End Property

            Public Property Fmid() As String
                Get
                    Return Me.fmidField
                End Get
                Set(ByVal value As String)
                    Me.fmidField = value
                End Set
            End Property

            Public Property Unp() As String
                Get
                    Return Me.unpField
                End Get
                Set(ByVal value As String)
                    Me.unpField = value
                End Set
            End Property

            Public Property CompanyId() As String
                Get
                    Return Me.companyIdField
                End Get
                Set(ByVal value As String)
                    Me.companyIdField = value
                End Set
            End Property

            Public Property DateOpened() As String
                Get
                    Return Me.dateOpenedField
                End Get
                Set(ByVal value As String)
                    Me.dateOpenedField = value
                End Set
            End Property

            Public Property DateIssued() As Date
                Get
                    Return Me.dateIssuedField
                End Get
                Set(ByVal value As Date)
                    Me.dateIssuedField = value
                End Set
            End Property

            Public Property OperType() As String
                Get
                    Return Me.operTypeField
                End Get
                Set(ByVal value As String)
                    Me.operTypeField = value
                End Set
            End Property

            Public Property ObjectID() As String
                Get
                    Return Me.objectIDField
                End Get
                Set(ByVal value As String)
                    Me.objectIDField = value
                End Set
            End Property

            Public Property PartnerId() As String
                Get
                    Return Me.partnerIdField
                End Get
                Set(ByVal value As String)
                    Me.partnerIdField = value
                End Set
            End Property

            Public Property UserId() As String
                Get
                    Return Me.userIdField
                End Get
                Set(ByVal value As String)
                    Me.userIdField = value
                End Set
            End Property

            Public Property Amount() As String
                Get
                    Return Me.amountField
                End Get
                Set(ByVal value As String)
                    Me.amountField = value
                End Set
            End Property

            Public Property AmountVat() As String
                Get
                    Return Me.amountVatField
                End Get
                Set(ByVal value As String)
                    Me.amountVatField = value
                End Set
            End Property

            Public Property PricesWithVat() As String
                Get
                    Return Me.pricesWithVatField
                End Get
                Set(ByVal value As String)
                    Me.pricesWithVatField = value
                End Set
            End Property

            Public Property NullVat() As String
                Get
                    Return Me.nullVatField
                End Get
                Set(ByVal value As String)
                    Me.nullVatField = value
                End Set
            End Property

            Public Property IsVat() As String
                Get
                    Return Me.isVatField
                End Get
                Set(ByVal value As String)
                    Me.isVatField = value
                End Set
            End Property

            Public Property PaymentTypeId() As String
                Get
                    Return Me.paymentTypeIdField
                End Get
                Set(ByVal value As String)
                    Me.paymentTypeIdField = value
                End Set
            End Property

            Public Property ItemId() As String
                Get
                    Return Me.itemIdField
                End Get
                Set(ByVal value As String)
                    Me.itemIdField = value
                End Set
            End Property

            Public Property Qtty() As String
                Get
                    Return Me.qttyField
                End Get
                Set(ByVal value As String)
                    Me.qttyField = value
                End Set
            End Property

            Public Property Discount() As String
                Get
                    Return Me.discountField
                End Get
                Set(ByVal value As String)
                    Me.discountField = value
                End Set
            End Property

            Public Property Price() As String
                Get
                    Return Me.priceField
                End Get
                Set(ByVal value As String)
                    Me.priceField = value
                End Set
            End Property

            Public Property RowSum() As String
                Get
                    Return Me.rowSumField
                End Get
                Set(ByVal value As String)
                    Me.rowSumField = value
                End Set
            End Property

            Public Property Note() As String
                Get
                    Return Me.noteField
                End Get
                Set(ByVal value As String)
                    Me.noteField = value
                End Set
            End Property

            Public Property PaymentId() As String
                Get
                    Return Me.paymentIdField
                End Get
                Set(ByVal value As String)
                    Me.paymentIdField = value
                End Set
            End Property

            Public Property AmountP() As String
                Get
                    Return Me.amountPField
                End Get
                Set(ByVal value As String)
                    Me.amountPField = value
                End Set
            End Property

            Public Property Total() As String
                Get
                    Return Me.totalField
                End Get
                Set(ByVal value As String)
                    Me.totalField = value
                End Set
            End Property

            Public Property FmidR() As String
                Get
                    Return Me.fmidRField
                End Get
                Set(ByVal value As String)
                    Me.fmidRField = value
                End Set
            End Property

            Public Property Ecrid() As String
                Get
                    Return Me.ecridField
                End Get
                Set(ByVal value As String)
                    Me.ecridField = value
                End Set
            End Property

            Public Property ReceiptId() As String
                Get
                    Return Me.receiptIdField
                End Get
                Set(ByVal value As String)
                    Me.receiptIdField = value
                End Set
            End Property

            Public Property ReceiptType() As String
                Get
                    Return Me.receiptTypeField
                End Get
                Set(ByVal value As String)
                    Me.receiptTypeField = value
                End Set
            End Property

            Public ReadOnly Property docDate() As Date
                Get
                    Return Me.docDateField
                End Get

            End Property

            Public ReadOnly Property UserRealTime() As String
                Get
                    Return Me.userRealTimeField
                End Get
            End Property

            Public ReadOnly Property Lot() As String
                Get
                    Return Me.lotField
                End Get
            End Property

            Public ReadOnly Property LotId() As Integer
                Get
                    Return Me.lotIdField
                End Get
            End Property

            Public ReadOnly Property CurrencyId() As Integer
                Get
                    Return Me.currencyIdField
                End Get
            End Property

            Public ReadOnly Property CurrencyRate() As Double
                Get
                    Return Me.currencyRateField
                End Get
            End Property

            Public ReadOnly Property SrcDocId() As Integer
                Get
                    Return Me.srcDocIdField
                End Get
            End Property

            Public ReadOnly Property PaymentType() As Integer
                Get
                    Return Me.paymentTypeField
                End Get
            End Property

            Public ReadOnly Property CurrentSign() As Integer
                Get
                    Return Me.currentSignField
                End Get
            End Property

            Public ReadOnly Property OperTypeForCashbook() As Integer
                Get
                    Return Me.operTypeForCashbookField
                End Get
            End Property

            Public ReadOnly Property DescriptionForCashbook() As String
                Get
                    Return Me.descriptionForCashbookField
                End Get
            End Property

        End Class

#End Region

#Region "Properties"

        Public Property Partner() As TPartner
            Get
                Return partnerStruct
            End Get
            Set(ByVal Value As TPartner)
                partnerStruct = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Създава сървиса и го стартира
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Public Sub StartServer()

            dbApplication = dbApplicationMain
            dbReplication = dbReplicationMain
            thisConfig = Config
            user = ActiveUser
            operation = New CustomLibrary.Operations.Sale(dbApplication, dbReplication, thisConfig)
            Dim localAddress As IPAddress = IPAddress.Parse("192.168.11.111")
            Dim tcpListener As New TcpListener(localAddress, 12345)
            tcpListener.Start()

            While True
                Dim client As TcpClient = tcpListener.AcceptTcpClient()
                Dim lock As New Object
                SyncLock lock
                    Using stream As NetworkStream = client.GetStream()
                        Request(stream)
                        Response(stream)
                        stream.Close()
                    End Using
                End SyncLock
            End While

        End Sub

#End Region

#Region "Private Methods"

#Region "Request Response Methods"

        ''' <summary>
        ''' Приема XML от клиента
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Sub Request(ByVal stream As NetworkStream)

            stringXML = New StringBuilder
            Dim requestBytes(100000) As Byte
            Dim readBytes As Int32 = stream.Read(requestBytes, 0, requestBytes.Length)
            Dim stringRequest As String = Encoding.UTF8.GetString(requestBytes, 0, readBytes)
            FormatRequest(stringRequest)

        End Sub

        ''' <summary>
        ''' Създава отговор към клиента
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Sub Response(ByVal stream As NetworkStream)
            Dim lock As New Object
            SyncLock lock
                'Dim xmlResponse As String = XmlCreateResponse()
                Dim xmlResponse As StringBuilder = stringXML
                Dim response As String = "HTTP/1.1 200 OK" + Environment.NewLine + Environment.NewLine + xmlResponse.ToString()
                Dim newResponse As String = response.Replace("&", "and")
                Dim newResponse2 As String = newResponse.Replace(",", ".")
                Dim responseBytes() As Byte = Encoding.UTF8.GetBytes(newResponse2)
                stream.Write(responseBytes, 0, responseBytes.Length)
            End SyncLock

        End Sub

        ''' <summary>
        ''' Чете XML фаила и го подава като string
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Function XmlCreateResponse() As String

            Dim result As String = ""

            If (IO.File.Exists("XmlDoc.xml")) Then

                Dim document As XmlReader = New XmlTextReader("XmlDoc.xml")
                While (document.Read())
                    Dim type As XmlNodeType = document.NodeType

                    If (type = XmlNodeType.Element) Then
                        result = document.ReadInnerXml.ToString()
                    End If

                End While
                document.Close()
            End If


            'Dim xmlSerializer As New XmlSerializer(GetType(Partners()))
            'Dim response As New StringBuilder
            'xmlSerializer.Serialize(New StringWriter(response), o)

            'Return response.ToString().TrimEnd()
            Return result
        End Function
        ''' <summary>
        ''' Разбива заявката на сектори и взима нужната информация
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Sub ParseQueryString(ByVal stringRequest As String)
            Dim lock As New Object
            SyncLock lock
                Dim newStringRequest As String = "https://appUrl.bg/Android/index.php?" & stringRequest
                Dim uri As New Uri(stringRequest) '"https://appUrl.bg/Android/index.php?Action=getCurrentUser&key=Собственик&Id=1"
                Dim action As String = HttpUtility.ParseQueryString(uri.Query).Get("Action")
                Dim username As String = HttpUtility.ParseQueryString(uri.Query).Get("key")
                Dim password As String = HttpUtility.ParseQueryString(uri.Query).Get("Id")
                Dim operationData As String = HttpUtility.ParseQueryString(uri.Query).Get("OperationData")
                Dim fiscalDeviceSN As String = HttpUtility.ParseQueryString(uri.Query).Get("FiscalDeviceSn")
                If Not CheckUser(username, password) Then
                    action = "incorrect username or password"
                End If
                CreateProccessByAction(action, username, operationData, fiscalDeviceSN)
            End SyncLock

        End Sub

        ''' <summary>
        ''' Проверява потребителя дали съществува
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>02.12.2019</date>
        Private Sub FormatRequest(ByVal request As String)

            Dim index As Integer = request.IndexOf("Action")
            Dim trimRequset As String = request.Substring(index)
            Dim newRequest As String = "https://appUrl.bg/Android/index.php?" & trimRequset

            ParseQueryString(newRequest)

        End Sub

        Private Sub ReplaceSymbols(ByVal stringXML As StringBuilder)

            'stringXML.ToString().Replace("&", "and")

        End Sub


        ''' <summary>
        ''' Проверява потребителя дали съществува
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>02.12.2019</date>
        Private Function CheckUser(ByVal username As String, ByVal password As String) As Boolean

            Dim userExist As Boolean = False

            Dim encryptPass As String = Encrypt(password, "CustomLibrary1111")
            'проверява дали има такъв потребител в базата
            userExist = user.DatabaseApplication.ExecuteScalar("SELECT ID FROM users WHERE NAME = ? AND Password = ?", New Object() {username, encryptPass})

            Return userExist

        End Function

        ''' <summary>
        ''' Определя каква операция трябва да се изпълни спрямо action-на на заявката
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>02.12.2019</date>
        Private Sub CreateProccessByAction(ByVal action As String, ByVal username As String, ByVal operationData As String, ByVal fiscalDeviceSN As String)

            Select Case action
                Case "saveOperation"
                    SaveSale(operationData)
                Case "getLastNumberForFiscalDevice"
                    LoadLastFiscalNumber(fiscalDeviceSN)
                Case "getCurrentUser"
                    LoadCurrentUser(username)
                Case "getCompanyInfo"
                    LoadCompanyInfo()
                Case "getObjects"
                    LoadObjects()
                Case "getPartners"
                    LoadPartners()
                Case "getPaymentButtons"
                    LoadPaymentTypes()
                Case "getItemGroups"
                    LoadItemGroups()
                Case "getItems"
                    LoadItems()
                Case "getLogin"
                    LoginAnswer()
                Case "getLogout"
                    LogoutAnswer()
                Case "getincorrect username or password"
                    IncorrectUserPass()
                Case Else

            End Select

        End Sub

#End Region

#Region "Help Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>27.02.2020</date>
        Private Function ConfigIsRegulation18() As Integer

            Dim isReg18 As Boolean = Config.isRegulation18
            Dim result As Integer

            If isReg18 Then
                result = 2
            Else
                result = 1
            End If

            Return result
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>27.02.2020</date>
        Private Function ConfigNegQtty() As Integer

            Dim isNegQTTY As Boolean = Config.NegativeQtty
            Dim result As Integer

            If isNegQTTY Then
                result = 2
            Else
                result = 1
            End If

            Return result
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>05.03.2020</date>
        Private Sub ChangeCultureInfo()

            Dim myCulture As CultureInfo = New CultureInfo("bg-BG")
            myCulture.NumberFormat.NumberDecimalSeparator = "."
            myCulture.NumberFormat.CurrencyDecimalSeparator = "."
            myCulture.NumberFormat.NegativeSign = "-"
            myCulture.DateTimeFormat.DateSeparator = "."
            myCulture.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy"
            myCulture.DateTimeFormat.LongDatePattern = "dd.MM.yyyy"
            myCulture.DateTimeFormat.TimeSeparator = ":"
            myCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss"
            myCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss"
            System.Threading.Thread.CurrentThread.CurrentCulture = myCulture

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>20.02.2020</date>
        Private Function TransactionNumberIsExist(ByVal transactionNumber As String) As String

            Dim SQL As String = "SELECT TransactionNumber FROM Payments WHERE TransactionNumber = '" & transactionNumber & "'"
            Dim IsExist As String = dbApplication.ExecuteScalar(SQL)
            If IsExist Is Nothing Then IsExist = " "
            Return IsExist
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>20.02.2020</date>
        Private Function GetHash(ByVal operationData As String) As String

            Using hasher As MD5 = MD5.Create()

                Dim dbytes As Byte() = hasher.ComputeHash(Encoding.UTF8.GetBytes(operationData))
                Dim sBuilder As New StringBuilder()

                For i As Integer = 0 To dbytes.Length - 1
                    sBuilder.Append(dbytes(i).ToString("X2"))
                Next i

                Return sBuilder.ToString()
            End Using

        End Function

        ''' <summary>
        ''' Взима IP на потребителя
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Function GetUserIp() As String
            Dim ip As String = String.Empty

            Try
                Dim webSites As String() = {"https://api.ipify.org", "https://icanhazip.com", "https://wtfismyip.com/text"}

                Using webClient As System.Net.WebClient = New System.Net.WebClient()

                    For Each webSite As String In webSites
                        If Not String.IsNullOrEmpty(webClient.DownloadString(webSite)) Then Return webClient.DownloadString(webSite)
                    Next
                End Using

            Catch ex As Exception
                Return String.Empty
            End Try

            Return ip
        End Function

        ''' <summary>
        ''' Криптира паролата на потребителя
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>29.11.2019</date>
        Private Shared Function Encrypt(ByVal strText As String, ByVal strEncrKey As String) As String
            Dim byKey() As Byte
            Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}

            Try
                byKey = System.Text.Encoding.UTF8.GetBytes(Left(strEncrKey, 8))

                Dim des As New DESCryptoServiceProvider
                Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(strText)
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write)
                cs.Write(inputByteArray, 0, inputByteArray.Length)
                cs.FlushFinalBlock()
                Return System.Convert.ToBase64String(ms.ToArray())

            Catch ex As Exception

                Return ex.Message
            End Try

        End Function

#End Region

#Region "Get operation data methods"

        Private Function GetOperData(ByVal operObject As OperationObject, ByVal data As XmlDocument) As OperationObject

            Dim opr As XmlNodeList = data.SelectNodes("/Root/Operation")
            For Each node As XmlNode In opr

                operObject.Id = node("id").InnerText
                operObject.FiscalDeviceSN = node("FiscalDeviceSN").InnerText
                operObject.Fmid = node("FMID").InnerText
                operObject.Unp = node("UNP").InnerText
                operObject.CompanyId = node("CompanyId").InnerText
                operObject.DateOpened = node("DateOpened").InnerText
                operObject.DateIssued = CDate(node("DateIssued").InnerText)
                operObject.OperType = node("OperType").InnerText
                operObject.ObjectID = node("ObjectId").InnerText
                operObject.PartnerId = node("PartnerId").InnerText
                operObject.UserId = node("UserId").InnerText
                operObject.Amount = node("Amount").InnerText
                operObject.AmountVat = node("AmountVat").InnerText
                operObject.PricesWithVat = node("PricesWithVat").InnerText
                operObject.NullVat = node("NullVat").InnerText
                operObject.IsVat = node("IsVat").InnerText
                operObject.PaymentTypeId = node("PaymentTypeId").InnerText

            Next

            Return operObject
        End Function

        Private Function GetPaymentData(ByVal operObject As OperationObject, ByVal data As XmlDocument) As OperationObject

            Dim pmt As XmlNodeList = data.SelectNodes("/Root/Operation/Payments/Payment")
            For Each node As XmlNode In pmt

                operObject.PaymentId = node("PaymentId").InnerText
                operObject.AmountP = node("Amount").InnerText

            Next

            Return operObject
        End Function

        Private Function GetECRReceiptData(ByVal operObject As OperationObject, ByVal data As XmlDocument) As OperationObject

            Dim ecr As XmlNodeList = data.SelectNodes("/Root/Operation/ECReceipt")
            For Each node As XmlNode In ecr

                operObject.Total = node("Total").InnerText
                operObject.FmidR = node("FMID").InnerText
                operObject.Ecrid = node("ECRID").InnerText
                operObject.ReceiptId = node("ReceiptID").InnerText
                operObject.ReceiptType = node("ReceiptType").InnerText

            Next

            Return operObject
        End Function

#End Region

#Region "Insert Methods"

        Private Sub InsertIntoOperationAndStore(ByVal operObject As OperationObject, ByVal Data As XmlDocument, ByVal acct As Integer, ByVal sign As Integer)


            Dim params As Object()
            Dim vat As Double = 0

            Dim itm As XmlNodeList = Data.SelectNodes("/Root/Operation/Items/Item")
            For Each node As XmlNode In itm
                operObject.ItemId = node("ItemId").InnerText
                operObject.Qtty = node("Qtty").InnerText
                operObject.Discount = node("Discount").InnerText
                operObject.Price = node("Price").InnerText
                operObject.RowSum = node("RowSum").InnerText
                operObject.Note = node("Note").InnerText
                vat = CDbl(operObject.Price) - (CDbl(operObject.Price) / (1 + Config.VAT()))

                'Dim a As String = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ToString()
                Dim SQL As String = "INSERT INTO operations" &
            " (OperType, Acct, PartnerID, ObjectID, OperatorID, GoodID, Qtty, Sign, PriceIn, PriceOut," &
            " VATIn, VATOut, Discount, CurrencyID, CurrencyRate, [Date], Lot, LotID, [Note], SrcDocID, UserID, UserRealTime) " &
            " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, '" & operObject.DateIssued.ToShortDateString & " " & operObject.DateIssued.ToShortTimeString & "', ?, ?, ?, ?, ?, " & operObject.UserRealTime & ")"

                params = New Object() _
                {CInt(operObject.OperType), acct, CInt(operObject.PartnerId), CInt(operObject.ObjectID), CInt(operObject.UserId), CInt(operObject.ItemId),
                 CDbl(operObject.Qtty), sign, CDbl(operObject.Price), CDbl(operObject.Price),
                 CDbl(vat), CDbl(vat), CDbl(operObject.Discount), operObject.CurrencyId, operObject.CurrencyRate,
                 operObject.Lot, operObject.LotId, operObject.Note, operObject.SrcDocId, CInt(operObject.UserId)}

                dbApplication.ExecuteNonQuery(SQL, params)

                'Dim SQLStore As String = "UPDATE store SET Qtty = Qtty + (" & qtty & ")" & _
                '" WHERE ObjectID = ? AND GoodID = ?"

                'params = New Object() {objectID, itemId}
                'dbApplication.ExecuteNonQuery(SQL, params)
            Next

        End Sub

        Private Sub InsertIntoPayments(ByVal operObject As OperationObject, ByVal data As XmlDocument, ByVal operationData As String, ByVal sign As Integer, ByVal acct As Integer)

            Dim params As Object()
            Dim transactionNumber As String = " "

            For i As Integer = 0 To 1
                If i = 1 Then
                    sign = 1
                End If
                If sign = 1 Then
                    transactionNumber = GetHash(operationData)
                    If TransactionNumberIsExist(transactionNumber).Length > 2 Then Throw New RepeatedOperationException
                End If

                Dim SqlPayment As String = " INSERT INTO payments (" &
                          " Acct, OperType, PartnerID, [Date], EndDate, [Mode], [Type], " &
                          " Qtty, TransactionNumber, UserID, ObjectID, [Sign], UserRealTime)" &
                          " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, " & operObject.UserRealTime & ")"

                params = New Object() _
                {acct, CInt(operObject.OperType), CInt(operObject.PartnerId), operObject.DateOpened, operObject.DateIssued, sign, operObject.PaymentType,
                 CDbl(operObject.AmountP), transactionNumber, CInt(operObject.UserId), CInt(operObject.ObjectID), operObject.CurrentSign}

                dbApplication.ExecuteNonQuery(SqlPayment, params)
                transactionNumber = " "
                sign = -1
            Next

        End Sub

        Private Sub InsertIntoCashbook(ByVal operObject As OperationObject)

            Dim params As Object()

            Dim SqlCashBook As String = "INSERT INTO cashbook ([Date], [Desc], OperType, Sign, Profit, ObjectID, UserID, UserRealTime)" &
                            " VALUES ('" & CDate(operObject.DateIssued).ToShortDateString & "', ?, ?, ?, ?, ?, ?, " & operObject.UserRealTime & ")"

            params = New Object() {Dictionary.GetResource("strAdvancePayment") & " - " & operObject.DescriptionForCashbook,
            operObject.OperTypeForCashbook, operObject.CurrentSign, CDbl(operObject.AmountP),
            CInt(operObject.ObjectID), CInt(operObject.UserId)}

            dbApplication.ExecuteNonQuery(SqlCashBook, params)

        End Sub

        Private Sub InsertIntoECRReceipts(ByVal operObject As OperationObject, ByVal acct As Integer)

            Dim params As Object()

            Dim tempReceiptType As Long = 8
            Dim SqlECRReceipts As String = "INSERT INTO ecrreceipts (OperType, Acct, ReceiptID, ReceiptDate, ReceiptType, ECRID, [Description], [Total], UserID, UserRealTime)" &
                " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, " & operObject.UserRealTime & ")"

            params = New Object() {CInt(operObject.OperType), CInt(acct), CInt(operObject.ReceiptId), CDate(operObject.DateIssued), tempReceiptType,
                                   operObject.Ecrid, operObject.Unp, CDbl(operObject.Total), CInt(operObject.UserId)}
            dbApplication.ExecuteNonQuery(SqlECRReceipts, params)


            Dim SqlECRReceipt As String = "INSERT INTO ecrreceipts (OperType, Acct, ReceiptID, ReceiptDate, ReceiptType, ECRID, [Description], [Total], UserID, UserRealTime)" &
                    " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, " & operObject.UserRealTime & ")"

            params = New Object() {CInt(operObject.OperType), CInt(acct), CInt(operObject.ReceiptId), CDate(operObject.DateIssued),
                                   CInt(operObject.ReceiptType), operObject.Ecrid, operObject.Unp, CDbl(operObject.Total), CInt(operObject.UserId)}
            dbApplication.ExecuteNonQuery(SqlECRReceipt, params)

        End Sub

#End Region

#Region "Proccess Methods"

        ''' <summary>
        ''' Записва операцията в базата
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date></date>
        Private Sub SaveSale(ByVal operationData As String)

            Dim lock As New Object
            SyncLock lock

                Dim operObject As New OperationObject
                Dim acct As Integer = 0
                Dim sign As Integer = -1
                Dim data As New XmlDocument()
                data.LoadXml(operationData)

                ChangeCultureInfo()

                operObject = GetOperData(operObject, data)
                operObject = GetPaymentData(operObject, data)
                operObject = GetECRReceiptData(operObject, data)

                Try

                    dbApplication.BeginTransaction()
                    ' Взема следващия номер на документ за текущата продажба
                    acct = operation.GetNextAcct(TOperType.Sale)
                    If acct = 0 Then dbApplication.RollBackTransaction()

                    InsertIntoOperationAndStore(operObject, data, acct, sign)
                    InsertIntoPayments(operObject, data, operationData, sign, acct)
                    InsertIntoCashbook(operObject)
                    InsertIntoECRReceipts(operObject, acct)

                    dbApplication.CommitTransaction()
                    stringXML.Append("<ROOT><STATUS>SUCCESS</STATUS><OPAERATION_ID></OPAERATION_ID><ACCT>" & acct & "</ACCT></ROOT>")
                Catch ex As RepeatedOperationException
                    stringXML.Append(ex.Message)
                Catch ex As Exception
                    dbApplication.RollBackTransaction()
                    stringXML.Append("<ROOT><STATUS>error</STATUS><OPAERATION_ID></OPAERATION_ID><ACCT>327</ACCT><ERROR_DESCRIPTION></ERROR_DESCRIPTION></ROOT>")
                End Try

            End SyncLock

        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>26.02.2020</date>
        Private Sub LoadLastFiscalNumber(ByVal fiscalDeviceSN As String)

            Dim SQL As String = "SELECT TOP 1 ReceiptID FROM ECRReceipts WHERE (OperType = 2 OR OperType = 1002 OR OperType = 2002 OR OperType = 3002) " &
            "AND ECRID = '" & fiscalDeviceSN & "' AND ReceiptType = 8 ORDER BY ReceiptID DESC"
            Dim lastReceiptID As String = CStr(dbApplication.ExecuteScalar(SQL))
            If lastReceiptID Is Nothing Then
                lastReceiptID = "0"
            End If
            stringXML.Append("<ROOT><STATUS>SUCCESS</STATUS><SN>" & fiscalDeviceSN & "</SN><LastNr>" & lastReceiptID & "</LastNr></ROOT>")

        End Sub

        ''' <summary>
        ''' Записва информация за текущия потребител в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadCurrentUser(ByVal username As String)

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "Users"
            Dim SQL As String = "SELECT ID, Code, Name, Name2, IsVeryUsed, GroupID, Password, UserLevel, [Deleted], CardNumber From " & tableName &
            " WHERE Name = '" & username & "'"
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<Root>")
            For Each row As DataRow In dataRow
                stringXML.Append("<CurrentUser>")
                stringXML.Append("<ID>" & row("ID").ToString() & "</ID>")
                stringXML.Append("<Code>" & row("Code").ToString() & "</Code>")
                stringXML.Append("<Name>" & row("Name").ToString() & "</Name>")
                stringXML.Append("<Name2>" & row("Name2").ToString() & "</Name2>")
                stringXML.Append("<IsVeryUsed>" & row("IsVeryUsed").ToString() & "</IsVeryUsed>")
                stringXML.Append("<GroupID>" & row("GroupID").ToString() & "</GroupID>")
                stringXML.Append("<Password>" & row("Password").ToString() & "</Password>")
                stringXML.Append("<UserLevel>" & row("UserLevel").ToString() & "</UserLevel>")
                stringXML.Append("<Deleted>" & row("Deleted").ToString() & "</Deleted>")
                stringXML.Append("<CardNumber>" & row("CardNumber").ToString() & "</CardNumber>")

                stringXML.Append("</CurrentUser>")
            Next
            stringXML.Append("</Root>")

        End Sub

        ''' <summary>
        ''' Записва информация за фирмата в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadCompanyInfo()

            Dim configH18 As Integer = ConfigIsRegulation18()
            Dim configNegativeQtty As Integer = ConfigNegQtty()
            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "Registration"
            Dim SQL As String = "SELECT ID, Code, Company, MOL, City, Address, Phone, Fax, eMail, TaxNo, Bulstat, BankName, BankCode, BankAcct, BankVATAcct," &
            " UserID, UserRealTime, IsDefault, Note1, Note2, [Deleted] FROM " & tableName & " WHERE [Deleted] = 0"
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<Root>")
            For Each row As DataRow In dataRow
                stringXML.Append("<Company>")
                stringXML.Append("<ID>" & row("ID").ToString() & "</ID>")
                stringXML.Append("<Code>" & row("Code").ToString() & "</Code>")
                stringXML.Append("<Company>" & row("Company").ToString() & "</Company>")
                stringXML.Append("<MOL>" & row("MOL").ToString() & "</MOL>")
                stringXML.Append("<City>" & row("City").ToString() & "</City>")
                stringXML.Append("<Address>" & row("Address").ToString() & "</Address>")
                stringXML.Append("<Phone>" & row("Phone").ToString() & "</Phone>")
                stringXML.Append("<Fax>" & row("Fax").ToString() & "</Fax>")
                stringXML.Append("<eMail>" & row("eMail").ToString() & "</eMail>")
                stringXML.Append("<TaxNo>" & row("TaxNo").ToString() & "</TaxNo>")
                stringXML.Append("<Bulstat>" & row("Bulstat").ToString() & "</Bulstat>")
                stringXML.Append("<BankName>" & row("BankName").ToString() & "</BankName>")
                stringXML.Append("<BankCode>" & row("BankCode").ToString() & "</BankCode>")
                stringXML.Append("<BankAcct>" & row("BankAcct").ToString() & "</BankAcct>")
                stringXML.Append("<BankVATAcct>" & row("BankVATAcct").ToString() & "</BankVATAcct>")
                stringXML.Append("<UserID>" & row("UserID").ToString() & "</UserID>")
                stringXML.Append("<UserRealTime>" & row("UserRealTime").ToString() & "</UserRealTime>")
                stringXML.Append("<IsDefault>" & row("IsDefault").ToString() & "</IsDefault>")
                stringXML.Append("<Note1>" & row("Note1").ToString() & "</Note1>")
                stringXML.Append("<Note2>" & row("Note2").ToString() & "</Note2>")
                stringXML.Append("<Deleted>" & row("Deleted").ToString() & "</Deleted>")
                stringXML.Append("<AllowNegativeQnt>" & configNegativeQtty.ToString() & "</AllowNegativeQnt>")
                stringXML.Append("<H18>" & configH18.ToString() & "</H18>")
                stringXML.Append("</Company>")
            Next
            stringXML.Append("</Root>")

        End Sub

        ''' <summary>
        ''' Записва номенклатура от обекти в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadObjects()

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "Objects"
            Dim SQL As String = "SELECT ID, Code, Name, Name2, PriceGroup, IsVeryUsed, GroupID, [Deleted] FROM " & tableName & " WHERE [Deleted] = 0"
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<Objects>")
            For Each row As DataRow In dataRow
                stringXML.Append("<Object>")
                stringXML.Append("<CompanyId>1</CompanyId>")
                stringXML.Append("<Code>" & row("Code").ToString() & "</Code>")
                stringXML.Append("<Name>" & row("Name").ToString() & "</Name>")
                stringXML.Append("<Address>London, New Barnet, str. Greenhill Park, 12</Address>")
                stringXML.Append("<PriceGroup>" & row("PriceGroup").ToString() & "</PriceGroup>")
                stringXML.Append("<Deleted>" & row("Deleted").ToString() & "</Deleted>")
                stringXML.Append("</Object>")
            Next
            stringXML.Append("</Objects>")

        End Sub

        ''' <summary>
        ''' Записва номенклатура от партньори в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>10.12.2019</date>
        Private Sub LoadPartners()

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "Partners"
            Dim SQL As String = "SELECT ID, partners.Code AS Code, Company, Company2, MOL, MOL2, City, City2, Address, Address2, Phone, Phone2, Fax, " &
                         "eMail, TaxNo, Bulstat, BankName, BankCode, BankAcct, BankVATName, BankVATCode, BankVATAcct, PriceGroup, " &
                         "Discount, Type, IsVeryUsed, UserID, UserRealTime, GroupID, CardNumber, Note1, Note2, PaymentDays, [Deleted] FROM " & tableName &
                         " WHERE [Deleted] = 0"
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<Partners>")
            For Each row As DataRow In dataRow
                stringXML.Append("<Partner>")
                stringXML.Append("<CompanyId>1</CompanyId>")
                stringXML.Append("<id>" & row("ID").ToString() & "</id>")
                stringXML.Append("<Code>" & row("ID").ToString() & "</Code>")
                stringXML.Append("<Company>" & row("Company").ToString() & "</Company>")
                stringXML.Append("<MOL>" & row("MOL").ToString() & "</MOL>")
                stringXML.Append("<City>" & row("City").ToString() & "</City>")
                stringXML.Append("<Address>" & row("Address").ToString() & "</Address>")
                stringXML.Append("<Phone>" & row("Phone").ToString() & "</Phone>")
                stringXML.Append("<eMail>" & row("eMail").ToString() & "</eMail>")
                stringXML.Append("<TaxNo>" & row("TaxNo").ToString() & "</TaxNo>")
                stringXML.Append("<Bulstat>" & row("Bulstat").ToString() & "</Bulstat>")
                stringXML.Append("<PriceGroup>" & row("PriceGroup").ToString() & "</PriceGroup>")
                stringXML.Append("<CardNumber>" & row("CardNumber").ToString() & "</CardNumber>")
                stringXML.Append("<Discount>" & row("Discount").ToString() & "</Discount>")
                stringXML.Append("<Type>" & row("Type").ToString() & "</Type>")
                stringXML.Append("<Deleted>" & row("Deleted").ToString() & "</Deleted>")
                stringXML.Append("</Partner>")
            Next
            stringXML.Append("</Partners>")

        End Sub

        ''' <summary>
        ''' Записва начините за плащане в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadPaymentTypes()

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "PaymentTypes"
            Dim SQL As String = "SELECT ID, Name, PaymentMethod FROM " & tableName
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<PaymentTypes>")

            stringXML.Append("<PaymentType>")
            stringXML.Append("<CompanyId>1</CompanyId>")
            stringXML.Append("<id>1</id>")
            stringXML.Append("<Name>Cash</Name>")
            stringXML.Append("<PaymentMethod>1</PaymentMethod>")
            stringXML.Append("<FiscalMode>2</FiscalMode>")
            stringXML.Append("</PaymentType>")

            stringXML.Append("<PaymentType>")
            stringXML.Append("<id>2</id>")
            stringXML.Append("<Name>Bank</Name>")
            stringXML.Append("<PaymentMethod>2</PaymentMethod>")
            stringXML.Append("<FiscalMode>2</FiscalMode>")
            stringXML.Append("</PaymentType>")

            stringXML.Append("<PaymentType>")
            stringXML.Append("<CompanyId>1</CompanyId>")
            stringXML.Append("<id>3</id>")
            stringXML.Append("<Name>Card</Name>")
            stringXML.Append("<PaymentMethod>3</PaymentMethod>")
            stringXML.Append("<FiscalMode>3</FiscalMode>")
            stringXML.Append("</PaymentType>")

            stringXML.Append("<PaymentType>")
            stringXML.Append("<CompanyId>1</CompanyId>")
            stringXML.Append("<id>4</id>")
            stringXML.Append("<Name>Voucher</Name>")
            stringXML.Append("<PaymentMethod>4</PaymentMethod>")
            stringXML.Append("<FiscalMode>1</FiscalMode>")
            stringXML.Append("</PaymentType>")

            stringXML.Append("</PaymentTypes>")

        End Sub

        ''' <summary>
        ''' Записва групите стоки в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadItemGroups()

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "GoodsGroups"
            Dim SQL As String = "SELECT ID, Name, Code FROM " & tableName
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            stringXML.Append("<ItemGroups>")
            For Each row As DataRow In dataRow
                stringXML.Append("<ItemGroup>")

                stringXML.Append("<CompanyId>1</CompanyId>")
                stringXML.Append("<id>" & row("ID").ToString() & "</id>")
                stringXML.Append("<Path>" & row("Code").ToString() & "</Path>")
                stringXML.Append("<Name>" & row("Name").ToString() & "</Name>")

                stringXML.Append("</ItemGroup>")
            Next
            stringXML.Append("</ItemGroups>")

        End Sub

        ''' <summary>
        ''' Записва номенклатура от стоки в XML фаил
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>07.01.2020</date>
        Private Sub LoadItems()

            NomenclatureDataset = New DataSet
            'Dim createXML As String = "XmlDoc.xml"
            Dim tableName As String = "Goods"
            Dim SQL As String = "SELECT ID, Code, Name, Name2, Barcode1, Barcode2, Barcode3, " &
            " Catalog1, Catalog2, Catalog3, Measure1, Measure2, Ratio, " &
            " PriceIn, PriceOut1, PriceOut2, PriceOut3, PriceOut4, PriceOut5, " &
            " PriceOut6, PriceOut7, PriceOut8, PriceOut9, PriceOut10, MinQtty, NormalQtty, Description, " &
            " Type, IsRecipe, TaxGroup, IsVeryUsed, GroupID, [Deleted] FROM " & tableName & " WHERE [Deleted] = 0"
            NomenclatureDataset = dbReplication.ExecuteDataset(SQL, tableName)
            'NomenclatureDataset.WriteXml(createXML)

            Dim dataRow As DataRowCollection = NomenclatureDataset.Tables(tableName).Rows

            stringXML.Append("<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>")
            stringXML.Append("<Items>")
            For Each row As DataRow In dataRow

                Dim pathID As String = dbApplication.ExecuteScalar("SELECT Code FROM GoodsGroups WHERE ID = " & row("GroupID"))

                stringXML.Append("<Item>")

                stringXML.Append("<id>" & row("ID").ToString() & "</id>")
                stringXML.Append("<Name>" & row("Name").ToString() & "</Name>")
                stringXML.Append("<TaxGroup>" & row("TaxGroup").ToString() & "</TaxGroup>")
                stringXML.Append("<Deleted>" & row("Deleted").ToString() & "</Deleted>")
                stringXML.Append("<TaxValue>15</TaxValue>")
                stringXML.Append("<PriceIn>" & row("PriceIn").ToString() & "</PriceIn>")
                stringXML.Append("<PriceOut1>" & row("PriceOut1").ToString() & "</PriceOut1>")
                stringXML.Append("<PriceOut2>" & row("PriceOut2").ToString() & "</PriceOut2>")
                stringXML.Append("<PriceOut3>" & row("PriceOut3").ToString() & "</PriceOut3>")
                stringXML.Append("<PriceOut4>" & row("PriceOut4").ToString() & "</PriceOut4>")
                stringXML.Append("<PriceOut5>" & row("PriceOut5").ToString() & "</PriceOut5>")
                stringXML.Append("<PriceOut6>" & row("PriceOut6").ToString() & "</PriceOut6>")
                stringXML.Append("<PriceOut7>" & row("PriceOut7").ToString() & "</PriceOut7>")
                stringXML.Append("<PriceOut8>" & row("PriceOut8").ToString() & "</PriceOut8>")
                stringXML.Append("<PriceOut9>" & row("PriceOut9").ToString() & "</PriceOut9>")
                stringXML.Append("<PriceOut10>" & row("PriceOut10").ToString() & "</PriceOut10>")
                stringXML.Append("<GroupPath>" & pathID & "</GroupPath>")
                stringXML.Append("<Path>" & row("Code").ToString() & "</Path>")
                stringXML.Append("<Description>" & row("Description").ToString() & "</Description>")
                stringXML.Append("<Photo></Photo>")
                stringXML.Append("<Codes>")
                stringXML.Append("<ItemCode>")
                stringXML.Append("<Code>" & row("Code").ToString() & "</Code>")
                stringXML.Append("<IsPrimary>1</IsPrimary>")
                stringXML.Append("<CodeType>2</CodeType>")
                stringXML.Append("<Ratio>" & row("Ratio").ToString() & "</Ratio>")
                stringXML.Append("<MeasureID>6</MeasureID>")
                stringXML.Append("<MeasureName>" & row("Measure1").ToString() & "</MeasureName>")
                stringXML.Append("</ItemCode>")
                stringXML.Append("</Codes>")

                stringXML.Append("</Item>")
            Next
            stringXML.Append("</Items>")

            ReplaceSymbols(stringXML)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>08.01.2020</date>
        Private Sub LoginAnswer()

            'Dim v As New XmlDocument()
            'v.AppendChild(v.CreateElement("item", "urn:1"))
            'v.Save("XmlDoc.xml")

            stringXML.Append(" ")

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>08.01.2020</date>
        Private Sub LogoutAnswer()

            'Dim v As New XmlDocument()
            'v.AppendChild(v.CreateElement("item", "urn:1"))
            'v.Save("XmlDoc.xml")

            stringXML.Append(" ")

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <developer>Симеон Иванов</developer>
        ''' <date>08.01.2020</date>
        Private Sub IncorrectUserPass()

            'Dim v As New XmlDocument()
            'v.AppendChild(v.CreateElement("item", "urn:1"))
            'v.Save("XmlDoc.xml")

            stringXML.Append(" ")

        End Sub

#End Region

#End Region


    End Class


