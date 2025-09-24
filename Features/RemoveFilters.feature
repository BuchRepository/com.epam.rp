Feature: Remove filters
  In order to organize my launches
  As a ReportPortal user
  I want to add, remove and manage filters

  Background:
    Given I am logged in as a valid user
    And I am on the Filters page

  Rule: A filter can be managed from Filters page

    Scenario Outline: User can remove a filter
      When I create a filter with name "<filterName>" and parameter "<parameter>" and quantity "<quantity>"
      And I delete the filter
      Then the filter should not be visible on the Filters page
      
      Examples:
        | filterName             | parameter      | quantity |
        | Automation bugs        | Automation Bug | 1        |
        | ProdBug                | Product Bug    | 1        |
#        | System issues          | System Issue   | 1        |
#        | To investigate group   | To Investigate | 1        |
#        | Launches with failures | Failed         | 1        |
