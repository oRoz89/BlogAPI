Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web.Http
Imports System.Web.Http.Description
Imports Newtonsoft.Json

Namespace Controllers
    Public Class PostsController
        Inherits ApiController

        Dim saconstring As String = " "
        Dim sacon As New SqlClient.SqlConnection(saconstring)
        Dim BazaCon As New SqlClient.SqlConnection


        ' GET: api/Posts
        Public Function GetValues() As String
            BazaCon.ConnectionString = saconstring
            Dim tblPost As DataTable = GetTable(BazaCon, "exec dbo.GetBlogPost '" & CreateSlug("augmented-reality-ios-application") & "'")

            Dim JSONresult As String = ""
            JSONresult = JsonConvert.SerializeObject(tblPost)
            Return JSONresult
        End Function

        ' GET: api/Posts/5
        Public Function GetValue(ByVal title As String) As String
            BazaCon.ConnectionString = saconstring
            Dim tblPost As DataTable = GetTable(BazaCon, "exec dbo.GetBlogPost '" & CreateSlug(title) & "'")

            Dim JSONresult As String = ""
            JSONresult = JsonConvert.SerializeObject(tblPost)
            Return JSONresult
        End Function

        ' POST: api/Posts
        Public Sub PostValue(<FromBody()> ByVal value As String)
            'test
        End Sub

        ' PUT: api/Posts/5
        Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

        End Sub

        ' DELETE: api/Posts/5
        Public Sub DeleteValue(ByVal id As Integer)

        End Sub


        <Route("api/Posts/{title}/post")>
        <ResponseType(GetType(String))>
        Public Function GetBlogPost(ByVal title As String)
            BazaCon.ConnectionString = saconstring
            Dim tblPost As DataTable = GetTable(BazaCon, "exec dbo.GetBlogPost '" & CreateSlug(title) & "'")
            Return Request.CreateResponse(HttpStatusCode.OK, tblPost)
            'Dim JSONresult As String = ""
            'JSONresult = JsonConvert.SerializeObject(tblPost)
            'Return JSONresult
        End Function


        <Route("api/Posts/{tag}/posts")>
        <ResponseType(GetType(String))>
        Public Function GetBlogPosts(ByVal tag As String)
            BazaCon.ConnectionString = saconstring
            Dim tblPosts As DataTable = GetTable(BazaCon, "exec dbo.GetBlogPosts '" & CreateSlug(tag) & "'")
            Return Request.CreateResponse(HttpStatusCode.OK, tblPosts)
        End Function

        <Route("api/Posts/tags")>
        <ResponseType(GetType(String))>
        Public Function GetTags()
            BazaCon.ConnectionString = saconstring
            Dim tblTags As DataTable = GetTable(BazaCon, "exec dbo.GetTagList")

            Dim Tagss As List(Of String) = New List(Of String)
            For Each item In tblTags.Rows
                Dim words() As String
                Dim semicolon() As Char = {";"c}
                words = item("tagList").Split(semicolon)
                Dim word As String

                For Each word In words
                    Dim a = word
                    Tagss.Add(word)
                Next
            Next
            Dim result As List(Of String) = Tagss.Distinct().ToList

            Dim JSONresult As String = ""
            JSONresult = JsonConvert.SerializeObject(result)

            Return Request.CreateResponse(HttpStatusCode.OK, tblTags)
        End Function

        <Route("api/Posts/insert/{title}/{description}/{body}/{taglist?}")>
        <ResponseType(GetType(String))>
        Public Function InsertPosts(title As String, description As String, body As String, Optional taglist As String = "") As String
            BazaCon.ConnectionString = saconstring

            Dim query As String = "INSERT INTO dbo.BlogPosts (slug, title, description, body, tagList, createdAt, updatedAt) VALUES ('[slug]', '[title]', '[description]', '[body]', '[tagList]', [createdAt], [updatedAt])"
            query = query.Replace("[slug]", CreateSlug(title))
            query = query.Replace("[title]", title)
            query = query.Replace("[description]", description)
            query = query.Replace("[body]", body)
            query = query.Replace("[tagList]", taglist)

            Dim dateNow As String = ConvertToDate(DateTime.Now,,, True)

            query = query.Replace("[createdAt]", dateNow)
            query = query.Replace("[updatedAt]", dateNow)

            Dim rezz As Integer = GetNonQuery(BazaCon, query)
            If rezz > 0 Then
                Return "OK"
            Else
                Return "ERROR"
            End If
        End Function

        <Route("api/Posts/update/{title_old?}/{title_new?}/{description?}/{body?}")>
        <ResponseType(GetType(String))>
        Public Function UpdatePost(ByVal title_old As String, Optional title_new As String = "", Optional description As String = "", Optional body As String = "") As String
            BazaCon.ConnectionString = saconstring

            Dim query As String = "UPDATE dbo.BlogPosts SET slug = '[slug]', title = '[title]' , description = '[description]', body = '[body]', updatedAt = [updateAt] WHERE slug = '[slug_old]'"
            If title_new.Length > 0 Then
                query = query.Replace("[slug]", CreateSlug(title_new))
                query = query.Replace("[title]", title_new)
            Else
                query = query.Replace("[slug]", CreateSlug(title_old))
                query = query.Replace("[title]", title_old)
            End If

            If description.Length = 0 Then
                query = query.Replace("description = '[description]',", "")
            Else
                query = query.Replace("[description]", description)
            End If
            If body.Length = 0 Then
                query = query.Replace("body = '[body]',", "")
            Else
                query = query.Replace("[body]", body)
            End If

            query = query.Replace("[updateAt]", ConvertToDate(DateTime.Now))

            query = query.Replace("[slug_old]", CreateSlug(title_old))

            Dim rez As Integer = GetNonQuery(BazaCon, query)
            If rez > 0 Then
                Return "OK"
            Else
                Return "NOTHING UPDATED"
            End If

            'http://localhost:57891/api/Posts/update?title_old=1&body=bodytest
            'http://localhost:57891/api/Posts/update?title_old=1&body=bodytest
        End Function


        <Route("api/Posts/delete/{title}")>
        <ResponseType(GetType(String))>
        Public Function DeletePost(ByVal title As String) As String
            BazaCon.ConnectionString = saconstring

            Dim query As String = "  DELETE FROM dbo.BlogPosts WHERE slug = '" & CreateSlug(title) & "'"
            Dim rezz As Integer = GetNonQuery(BazaCon, query)
            If rezz > 0 Then
                Return "OK"
            Else
                Return "NOTHING WAS DELETED"
            End If

        End Function

        Public Function CreateSlug(ByVal Title As String) As String
            Dim Slug As String = Title.ToLower()
            Slug = Slug.Replace("ć", "c")
            Slug = Slug.Replace("š", "s")
            Slug = Slug.Replace("ž", "z")
            Slug = Slug.Replace("đ", "dj")
            Slug = Slug.Replace("-", " ")
            Slug = Regex.Replace(Slug, "[^a-z0-9\s-]", " ")
            Slug = Regex.Replace(Slug, "\s+", " ").Trim()
            Slug = Slug.Replace(" ", "-")

            Return Slug
        End Function


        Public Shared Function GetTable(ByVal Connection As SqlConnection, ByVal SQL As String, Optional ByVal ShowError As Boolean = False) As DataTable
            Dim ps As ConnectionState = Connection.State
            Dim tbl As New DataTable
            Dim cmd As New SqlDataAdapter(SQL, Connection)
            cmd.SelectCommand.CommandTimeout = 0
            Try
                Connection.Close()
                If ps <> ConnectionState.Open Then Connection.Open()
                cmd.Fill(tbl)
            Catch ex As Exception
            End Try
            Return tbl
        End Function


        Public Shared Function GetNonQuery(ByVal Connection As SqlConnection, ByVal SQL As String, Optional ByVal ShowError As Boolean = False, Optional ByVal LogException As Boolean = False) As Object

            Dim ps As ConnectionState = Connection.State

            Dim dr As Object = Nothing
            Dim cmd As New SqlCommand(SQL, Connection)
            cmd.CommandTimeout = 0

            If ps <> ConnectionState.Open Then Connection.Open()

            Try
                dr = cmd.ExecuteNonQuery
                cmd = Nothing
            Catch ex As Exception

            End Try
            cmd = Nothing
            Return dr
        End Function

        Public Shared Function ConvertToDate(ByVal dt As String, Optional ByVal EndDate As Boolean = False, Optional ByVal tm As String = "", Optional ByVal WithTime As Boolean = False, Optional ByVal StartDate As Boolean = False) As String
            Dim d As Date = CType(dt, Date)
            Dim str, mon, day As String
            'str = d.Year.ToString
            If d.Month < 10 Then
                mon = "0" & d.Month.ToString
            Else
                mon = d.Month.ToString
            End If

            If d.Day < 10 Then
                day = "0" & d.Day.ToString
            Else
                day = d.Day.ToString
            End If

            str = d.Year.ToString & mon & day
            If EndDate Then
                Return "'" & str & " 23:59'"
            ElseIf StartDate Then
                Return "'" & str & " 00:00'"
            Else
                If tm <> "" Then
                    Return "'" & str & " " & tm & "'"
                Else
                    If WithTime = True Then
                        Return "'" & str & " " & d.Hour & ":" & d.Minute & ":" & d.Second & ".000" & "'"
                    Else
                        Return "'" & str & "'"
                    End If

                End If
            End If
        End Function

    End Class
End Namespace