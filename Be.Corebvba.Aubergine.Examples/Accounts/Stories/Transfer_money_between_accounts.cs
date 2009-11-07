using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Examples.Accounts.Contexts;

namespace Be.Corebvba.Aubergine.Examples.Accounts.Stories
{
    class Transfer_money_between_accounts : Story<AccountContext>
    {
        As_a user;
        I_want to_transfer_money_between_accounts;
        So_that I_can_have_real_use_for_my_money;

        Given AccountA_has_3_m;
        Given AccountB_has_2_m;

        [Cols("xx", "yy", "zz")]
        [Data(1, 2, 3)]
        [Data(2, 1, 4)]
        [Data(3, 0, 5)]
        class Transfer_xx_m_between_2_accounts : Scenario
        {
            Given the_current_user_is_authenticated_for_AccountA;
            When transfering_xx_m_from_AccountA_to_AccountB;
            Then it_should_have_yy_m_on_AccountA;
            Then it_should_have_zz_m_on_AccountB;
        }

        class Transfer_too_much : Scenario
        {
            Given the_current_user_is_authenticated_for_AccountA;
            When transfering_4_m_from_AccountA_to_AccountB;
            Then it_should_have_3_m_on_AccountA;
            Then it_should_have_2_m_on_AccountB;
            Then it_should_fail_with_error;
        }

        class Not_authorized_for_transfer : Scenario
        {
            When transfering_1_m_from_AccountB_to_AccountA;
            Then it_should_have_3_m_on_AccountA;
            Then it_should_have_2_m_on_AccountB;
            Then it_should_fail_with_error;
        }
    }
}
