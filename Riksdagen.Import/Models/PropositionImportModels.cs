
using System;
using System.Collections.Generic;

public class Aktivitet
{
    public string kod { get; set; }
    public string namn { get; set; }
    public string datum { get; set; }
    public string status { get; set; }
    public string ordning { get; set; }
    public string process { get; set; }
}

public class Dokaktivitet
{
    public List<Aktivitet> aktivitet { get; set; }
}

public class Intressent
{
    public string intressent_id { get; set; }
    public string namn { get; set; }
    public string partibet { get; set; }
    public string ordning { get; set; }
    public string roll { get; set; }
}

public class Dokintressent
{
    public List<Intressent> intressent { get; set; }
}

public class Uppgift
{
    public string kod { get; set; }
    public string namn { get; set; }
    public string text { get; set; }
    public string dok_id { get; set; }
    public string systemdatum { get; set; }
}

public class Dokuppgift
{
    public List<Uppgift> uppgift { get; set; }
}

public class Bilaga
{
    public string dok_id { get; set; }
    public object subtitel { get; set; }
    public string filnamn { get; set; }
    public string filstorlek { get; set; }
    public string filtyp { get; set; }
    public string titel { get; set; }
    public string fil_url { get; set; }
}

public class Dokbilaga
{
    public Bilaga bilaga { get; set; }
}

public class Referens
{
    public string referenstyp { get; set; }
    public string uppgift { get; set; }
    public string ref_dok_id { get; set; }
    public string ref_dok_typ { get; set; }
    public string ref_dok_rm { get; set; }
    public string ref_dok_bet { get; set; }
    public string ref_dok_titel { get; set; }
    public object ref_dok_subtitel { get; set; }
    public string ref_dok_subtyp { get; set; }
    public string ref_dok_dokumentnamn { get; set; }
}

public class Dokreferens
{
    public Referens referens { get; set; }
}

public class Dokument
{
    public string hangar_id { get; set; }
    public string dok_id { get; set; }
    public string rm { get; set; }
    public string beteckning { get; set; }
    public string typ { get; set; }
    public string subtyp { get; set; }
    public string doktyp { get; set; }
    public string typrubrik { get; set; }
    public string dokumentnamn { get; set; }
    public string debattnamn { get; set; }
    public string tempbeteckning { get; set; }
    public string organ { get; set; }
    public string mottagare { get; set; }
    public string nummer { get; set; }
    public string slutnummer { get; set; }
    public string datum { get; set; }
    public string systemdatum { get; set; }
    public string publicerad { get; set; }
    public string titel { get; set; }
    public string subtitel { get; set; }
    public string status { get; set; }
    public string htmlformat { get; set; }
    public string relaterat_id { get; set; }
    public string source { get; set; }
    public string sourceid { get; set; }
    public string metadata { get; set; }
    public string dokument_url_text { get; set; }
    public string dokument_url_html { get; set; }
    public string dokumentstatus_url_xml { get; set; }
    public string html { get; set; }
}

public class Dokumentstatus
{
    public Dokument dokument { get; set; }
  //  public Dokaktivitet dokaktivitet { get; set; }
//    public Dokintressent dokintressent { get; set; }
 //   public Dokuppgift dokuppgift { get; set; }
//    public Dokbilaga dokbilaga { get; set; }
  //  public Dokreferens dokreferens { get; set; }

}

public class PropJson
{
    public Dokumentstatus dokumentstatus { get; set; }
}