Feature: Filters Management
  In order to organize my launches
  As a ReportPortal user
  I want to add, remove and manage filters

  Background:
    Given I am logged in as a valid user
    And I am on the Filters page

  Rule: A filter can be managed from Filters page

    Scenario Outline: User can add a filter
      When I add a filter with name "<filterName>" and parameter "<parameter>" and quantity "<quantity>"
      Then the filter "<filterName>" should be visible on the Filters page
      And I remove the filter "<filterName>"

      Examples:
        | filterName | parameter      | quantity |
        | AutoBug    | Automation Bug | 1        |
        | ProdBug    | Product Bug    | 1        |

    Scenario: User can remove a filter
      When I create a filter "TempFilter" with parameter "System Issue" and quantity "1"
      And I delete the filter "TempFilter"
      Then the filter "TempFilter" should not be visible on the Filters page

    Scenario: User can toggle filter display
      When I create a filter "DisplayFilter" with parameter "More" and quantity "1"
      And I toggle display of "DisplayFilter"
      Then the filter "DisplayFilter" should not be visible on Launches page
      But it should still exist in the Filters page
      And I remove the filter "DisplayFilter"
