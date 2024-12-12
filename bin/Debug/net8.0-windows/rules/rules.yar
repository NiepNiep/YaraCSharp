rule ExampleRule  {
    strings:
        $mytext = "malicious"
    condition:
        $mytext
}