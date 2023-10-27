<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs"
    xmlns="http://schemas.microsoft.com/wix/2006/wi">

  <xsl:output method="xml" indent="yes" />

  <xsl:key
    name="ExeToRemove"
    match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.exe' ]"
    use="@Id" />
  <xsl:key
    name="PdbToRemove"
    match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.pdb' ]"
    use="@Id" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>
  <xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'ExeToRemove', @Id ) ]" />
  <xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'PdbToRemove', @Id ) ]" />

</xsl:stylesheet>