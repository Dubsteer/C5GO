-- Sanitized demonstration data for the supervisor package.
-- All names, email addresses and SteamID64 values in this file are fictional.

START TRANSACTION;

INSERT INTO `user`
    (id, first_name, last_name, birthday, age, username, email, password,
     is_moderator, steam_id, show_steam_profile, email_confirmed,
     email_token, token_created_at)
VALUES
    (1, 'Demo', 'Administrator', '1985-04-12', 41, 'profesor',
     'profesor@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     1, '76561198000020001', 0, 1, NULL, NULL),
    (2, 'Marko', 'Kovacevic', '1999-02-18', 27, 'marko.igl',
     'marko.igl@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020002', 0, 1, NULL, NULL),
    (3, 'Ana', 'Jovanovic', '2001-11-03', 24, 'ana.awp',
     'ana.awp@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020003', 0, 1, NULL, NULL),
    (4, 'Nikola', 'Radic', '2000-06-24', 26, 'nikola.entry',
     'nikola.entry@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020004', 0, 1, NULL, NULL),
    (5, 'Milica', 'Savic', '2002-01-30', 24, 'milica.support',
     'milica.support@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020005', 0, 1, NULL, NULL),
    (6, 'Petar', 'Lukic', '1998-09-14', 27, 'petar.lurk',
     'petar.lurk@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020006', 0, 1, NULL, NULL),
    (7, 'Sara', 'Ilic', '2003-03-08', 23, 'sara.clutch',
     'sara.clutch@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020007', 0, 1, NULL, NULL),
    (8, 'Luka', 'Vukovic', '1997-12-19', 28, 'luka.anchor',
     'luka.anchor@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020008', 0, 1, NULL, NULL),
    (9, 'Jelena', 'Matic', '2001-05-11', 25, 'jelena.rifle',
     'jelena.rifle@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020009', 0, 1, NULL, NULL),
    (10, 'Ivan', 'Todorovic', '1996-08-02', 30, 'ivan.strat',
     'ivan.strat@c5g0.local', '$2a$12$qhIvF79MS6sGg6ohmPT.0eRCyGkuVdwbWwIW3Bsw.uCaKeWu0ek4C',
     0, '76561198000020010', 0, 1, NULL, NULL);

INSERT INTO user_role (user_id, role_id, assigned_by, assigned_at, reason)
SELECT id, 1, NULL, '2026-08-20 12:00:00', 'Demonstration member account'
FROM `user`;

INSERT INTO user_role (user_id, role_id, assigned_by, assigned_at, reason)
VALUES (1, 4, NULL, '2026-08-20 12:00:00', 'Demonstration owner account');

INSERT INTO role_assignment_audit
    (user_id, role_id, action_type, performed_by, reason, created_at)
VALUES
    (1, 4, 0, NULL, 'Demonstration owner account', '2026-08-20 12:00:00');

INSERT INTO team (id, name, captain_id, created_at)
VALUES
    (1, 'Adriatic Vortex', 2, '2026-08-18 18:00:00'),
    (2, 'Balkan Pulse', 7, '2026-08-18 18:30:00');

INSERT INTO team_player (team_id, user_id, role, status)
VALUES
    (1, 2, 'Captain', 'Approved'),
    (1, 3, 'Member', 'Approved'),
    (1, 4, 'Member', 'Approved'),
    (1, 5, 'Member', 'Approved'),
    (1, 6, 'Member', 'Approved'),
    (2, 7, 'Captain', 'Approved'),
    (2, 8, 'Member', 'Approved'),
    (2, 9, 'Member', 'Approved'),
    (2, 10, 'Member', 'Approved'),
    (2, 1, 'Member', 'Approved');

INSERT INTO tournament
    (id, name, description, status_int, is_team, team_size_required)
VALUES
    (1, 'C5GO Autumn Cup 2026',
     'Zavrseni individualni turnir sa osam igraca i kompletnim eliminacionim bracketom.',
     2, 0, 1),
    (2, 'Weekend Aim Challenge',
     'Otvoreni individualni turnir za igrace koji zele da testiraju formu tokom vikenda.',
     0, 0, 1),
    (3, 'Balkan Team Series',
     'Timski turnir pet na pet pripremljen za buduce prijave ekipa.',
     0, 1, 5);

INSERT INTO applications (tournamentId, playerid)
VALUES
    (1, 2), (1, 3), (1, 4), (1, 5),
    (1, 6), (1, 7), (1, 8), (1, 9),
    (2, 3), (2, 4), (2, 7), (2, 8);

INSERT INTO matches
    (id, tournamentId, user_id1, user_id2, player1Score, player2Score,
     match_date, status_int, round_number, bracket_position)
VALUES
    (1, 1, 2, 3, 13, 9,  '2026-08-20 18:00:00', 2, 1, 1),
    (2, 1, 4, 5, 8,  13, '2026-08-20 19:00:00', 2, 1, 2),
    (3, 1, 6, 7, 13, 11, '2026-08-20 20:00:00', 2, 1, 3),
    (4, 1, 8, 9, 10, 13, '2026-08-20 21:00:00', 2, 1, 4),
    (5, 1, 2, 5, 13, 7,  '2026-08-21 18:00:00', 2, 2, 1),
    (6, 1, 6, 9, 12, 13, '2026-08-21 19:00:00', 2, 2, 2),
    (7, 1, 2, 9, 13, 10, '2026-08-22 20:00:00', 2, 3, 1);

INSERT INTO post (id, authorid, content, posted_on, title, image_path)
VALUES
    (1, 1,
     'Autumn Cup je zavrsen nakon tri runde. Marko je u finalu savladao Jelenu rezultatom 13:10, a kompletan bracket dostupan je na stranici turnira.',
     '2026-08-22 21:15:00', 'C5GO Autumn Cup je zavrsen', NULL),
    (2, 1,
     'Prijave za Weekend Aim Challenge su otvorene. Za ucesce je potreban potvrden nalog i ispravan SteamID64.',
     '2026-08-23 10:00:00', 'Otvorene prijave za Weekend Aim Challenge', NULL),
    (3, 1,
     'Timski turniri koriste postave od pet odobrenih igraca. Svaki clan ekipe mora imati ispravan SteamID64 prije prijave.',
     '2026-08-24 12:30:00', 'Pravila za timske turnire', NULL);

INSERT INTO comment (id, authorid, content, posted_on, post_id)
VALUES
    (1, 3, 'Finale je bilo odlicno, posebno posljednjih nekoliko rundi.',
     '2026-08-22 21:30:00', 1),
    (2, 7, 'Prijava je prosla bez problema. Vidimo se na serveru.',
     '2026-08-23 10:20:00', 2);

INSERT INTO commentreply (id, content, posted_on, comment_id, user_id)
VALUES
    (1, 'Hvala! Rezultat i svi mecevi ostaju dostupni u istoriji.',
     '2026-08-22 21:45:00', 1, 1);

INSERT INTO notification (id, user_id, message, link, is_read, created_at)
VALUES
    (1, 2, 'Cestitamo, osvojili ste C5GO Autumn Cup 2026.', '/Tournaments/Details?id=1', 0,
     '2026-08-22 20:45:00'),
    (2, 3, 'Prijava za Weekend Aim Challenge je evidentirana.', '/Tournaments/Details?id=2', 0,
     '2026-08-23 10:10:00'),
    (3, 1, 'Objavljen je rezultat finalnog meca.', '/Matches/History', 1,
     '2026-08-22 21:00:00');

INSERT INTO discussion
    (id, author_id, category_id, title, content, image_path, youtube_video_id,
     is_spoiler, is_locked, is_pinned, status_int, created_at)
VALUES
    (1, 3, 1, 'Koju mapu najvise trenirate?',
     'Posljednjih dana najvise igram Ancient. Koju mapu biste preporucili ekipi koja tek pocinje zajednicke treninge?',
     NULL, NULL, 0, 0, 1, 0, '2026-08-23 16:00:00'),
    (2, 9, 4, 'Utisci sa finala Autumn Cupa',
     'Finale je odluceno u zavrsnici, a Marko je sacuvao prednost u posljednje dvije runde.',
     NULL, NULL, 1, 0, 0, 0, '2026-08-22 21:10:00'),
    (3, 6, 3, 'Kako organizujete trening ekipe?',
     'Mi odvajamo jedan dan za utility, a dva dana za scrim meceve. Zanima me kako ostale ekipe prave raspored.',
     NULL, NULL, 0, 0, 0, 0, '2026-08-24 18:30:00');

INSERT INTO discussion_vote (discussion_id, user_id, vote_value, created_at)
VALUES
    (1, 2, 1, '2026-08-23 16:10:00'),
    (1, 5, 1, '2026-08-23 16:12:00'),
    (2, 3, 1, '2026-08-22 21:20:00'),
    (3, 7, 1, '2026-08-24 18:40:00'),
    (3, 8, 1, '2026-08-24 18:45:00');

INSERT INTO discussion_comment
    (id, discussion_id, author_id, parent_comment_id, content, status_int, created_at)
VALUES
    (1, 1, 2, NULL,
     'Za novu ekipu bih prvo izabrao Mirage, a onda dodao Ancient kada se dogovore osnovne pozicije.',
     0, '2026-08-23 16:20:00'),
    (2, 1, 5, 1,
     'Slazem se. Mirage je dobar za komunikaciju i jednostavnije dogovaranje utilityja.',
     0, '2026-08-23 16:35:00'),
    (3, 3, 7, NULL,
     'Kod nas su ponedjeljak i srijeda za scrim, a petkom pregledamo demo snimke.',
     0, '2026-08-24 19:00:00');

INSERT INTO discussion_comment_vote (comment_id, user_id, vote_value, created_at)
VALUES
    (1, 3, 1, '2026-08-23 16:25:00'),
    (1, 4, 1, '2026-08-23 16:27:00'),
    (3, 6, 1, '2026-08-24 19:05:00');

COMMIT;
