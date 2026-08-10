# Web feature parity

The website is the only active user interface for C5GO. The former Windows application was removed after its relevant administration workflows were verified on the website.

| Capability | Website replacement |
| --- | --- |
| Administrator login and access control | `/Login` and the `AdminOnly` authorization policy |
| User list, search and account deletion | `/Admin/Users` |
| Player profile removal | `/Admin/Users` |
| Post creation, editing and deletion | `/Admin/Posts` |
| Tournament creation, filtering and deletion | `/Admin/Tournaments` |
| Tournament settings and status | `/Admin/Tournaments/Details` |
| Solo participant management | `/Admin/Tournaments/Details` |
| Team registration management | `/Admin/Tournaments/Details` |
| Solo and team bracket generation | `/Admin/Tournaments/Details` |
| Solo and team match result editing | `/Admin/Tournaments/Details` |
| Public tournament participants and matches | `/Tournaments/Details` |

The desktop-only automatic team fill action was intentionally not migrated. It assigned available users to teams without a membership request and bypassed the website's captain approval workflow. Team membership remains controlled by user requests and captain decisions.

The separate desktop bracket window was a presentation layer over the same opening-round match data. The website exposes those matches through both administration and public tournament pages.
