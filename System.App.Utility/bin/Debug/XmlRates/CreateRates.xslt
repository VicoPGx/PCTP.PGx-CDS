<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet  version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:gesmes="http://www.gesmes.org/xml/2002-08-01" 
  xmlns=""
  >

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/" xml:space="default">
    <Rates>
    <xsl:for-each select="gesmes:Envelope/*/*/*">
      <Rate>
        <Currency>
          <xsl:value-of select="@currency"/>
        </Currency>
        <Exchange>
          <xsl:value-of select="@rate"/>
        </Exchange>
        <Symbol>
          <xsl:value-of select="@currency"/>
        </Symbol>
      </Rate>
    </xsl:for-each>
    </Rates>
  </xsl:template>

</xsl:stylesheet>