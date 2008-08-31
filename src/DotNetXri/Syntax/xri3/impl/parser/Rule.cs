package org.openxri.xri3.impl.parser;


import java.util.ArrayList;

public abstract class Rule
{
  public final String spelling;
  public final ArrayList rules;

  protected Rule(String spelling, ArrayList rules)
  {
    this.spelling = spelling;
    this.rules = rules;
  }

  public Rule(Rule rule)
  {
    this(rule.spelling, rule.rules);
  }

  public String toString()
  {
    return spelling;
  }

  public bool equals(Object object)
  {
    return object instanceof Rule && spelling.equals(((Rule)object).spelling);
  }

  public int hashCode()
  {
    return spelling.hashCode();
  }

  public int compareTo(Rule rule)
  {
    return spelling.compareTo(rule.spelling);
  }

  public abstract Object visit(Visitor visitor);
}

/* -----------------------------------------------------------------------------
 * eof
 * -----------------------------------------------------------------------------
 */
