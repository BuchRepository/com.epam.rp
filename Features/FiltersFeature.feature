Feature: Filters Management
  In order to organize my launches
  As a ReportPortal user
  I want to add, remove and manage filters

  Background:
    Given I am logged in as a valid user
    And I am on the Filters page

  Rule: A filter can be managed from Filters page

    Scenario Outline: User can add a filter
      When I create a filter with name "<filterName>" and parameter "<parameter>" and quantity "<quantity>"
      Then the filter "<filterName>" should be visible on the Filters page
      And I delete the filter "<filterName>"

      Examples:
        | filterName | parameter      | quantity |
        | AutoBug    | Automation Bug | 1        |
        | ProdBug    | Product Bug    | 1        |

    Scenario Outline: User can remove a filter
      When I create a filter with name "<filterName>" and parameter "<parameter>" and quantity "<quantity>"
      And I delete the filter "<filterName>"
      Then the filter "<filterName>" should not be visible on the Filters page
      
      Examples:
        | filterName | parameter      | quantity |
        | AutoBug    | Automation Bug | 1        |
        | ProdBug    | Product Bug    | 1        |

    Scenario Outline: User can toggle filter display
      When II create a filter with name "<filterName>" and parameter "<parameter>" and quantity "<quantity>"
      And I toggle display of "DisplayFilter"
      Then the filter "<filterName>" should not be visible on the Launches page
      But it should still exist in the Filters page
      And I delete the filter "<filterName>"

      Examples:
        | filterName | parameter      | quantity |
        | AutoBug    | Automation Bug | 1        |
        | ProdBug    | Product Bug    | 1        |
