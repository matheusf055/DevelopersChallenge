// Wait for the DOM to load
document.addEventListener('DOMContentLoaded', () => {
    // Get forms for team, tournament, and match
    const teamForm = document.getElementById('teamForm');
    const tournamentForm = document.getElementById('tournamentForm');
    const matchForm = document.getElementById('matchForm');

    // Handle team form submission
    if (teamForm) {
        teamForm.addEventListener('submit', async (e) => {
            e.preventDefault();  // Prevent default form submission
            const teamName = document.getElementById('teamName').value;
            const regionName = document.getElementById('regionName').value;

            try {
                // Send POST request to register a new team
                const response = await fetch('http://localhost:5000/api/team', { 
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'  // Set content type to JSON
                    },
                    body: JSON.stringify({ name: teamName, region: regionName })  // Convert data to JSON
                });

                if (response.ok) {
                    alert('Team registered successfully!');  // Success alert
                    teamForm.reset();  // Reset form fields
                } else {
                    alert('Error registering the team.');  // Error alert
                }
            } catch (error) {
                console.error('Error:', error);  // Log error
                alert('Error registering the team.');  // Error alert
            }
        });
    }

    // Fetch and display all teams
    async function getAllTeams() {
        try {
            const response = await fetch('http://localhost:5000/api/team');
            if (response.ok) {
                const data = await response.json();  // Parse response JSON
                const teamItems = document.getElementById('teamItems');  // Get element for team list
                teamItems.innerHTML = '';  // Clear existing content
                data.forEach(team => {
                    const li = document.createElement('li');  // Create new list item
                    li.textContent = `ID: ${team.id}, Name: ${team.name}, Region: ${team.region}`;  // Set text content
                    teamItems.appendChild(li);  // Add list item to the list
                });
            } else {
                showAlert('Error fetching teams.', 'error');  // Error alert
            }
        } catch (error) {
            console.error('Error:', error);  // Log error
            showAlert('Error fetching teams.', 'error');  // Error alert
        }
    }

    // Handle tournament form submission
    if (tournamentForm) {
        tournamentForm.addEventListener('submit', async (e) => {
            e.preventDefault();  // Prevent default form submission
            const tournamentName = document.getElementById('tournamentName').value;
            const startDate = document.getElementById('startDate').value;

            try {
                // Send POST request to register a new tournament
                const response = await fetch('http://localhost:5000/api/tournament', { 
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'  // Set content type to JSON
                    },
                    body: JSON.stringify({ name: tournamentName, startdate: startDate })  // Convert data to JSON
                });

                if (response.ok) {
                    alert('Tournament registered successfully!');  // Success alert
                    tournamentForm.reset();  // Reset form fields
                } else {
                    alert('Error registering the tournament.');  // Error alert
                }
            } catch (error) {
                console.error('Error:', error);  // Log error
                alert('Error registering the tournament.');  // Error alert
            }
        });
    }

    // Fetch and display all tournaments
    async function getAllTournaments() {
        try {
            const response = await fetch('http://localhost:5000/api/tournament');
            if (response.ok) {
                const data = await response.json();  // Parse response JSON
                const tournamentItems = document.getElementById('tournamentItems');  // Get element for tournament list
                tournamentItems.innerHTML = '';  // Clear existing content
                data.forEach(tournament => {
                    const id = tournament.id || 'Unknown';  // Handle missing IDs
                    const name = tournament.name || 'No name';  // Handle missing names
                    const startDate = tournament.startDate ? new Date(tournament.startDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' }) : 'Date not available';  // Format date

                    const li = document.createElement('li');  // Create new list item
                    li.textContent = `ID: ${id}, Name: ${name}, Start Date: ${startDate}`;  // Set text content
                    tournamentItems.appendChild(li);  // Add list item to the list
                });
            } else {
                showAlert('Error fetching tournaments.', 'error');  // Error alert
            }
        } catch (error) {
            console.error('Error:', error);  // Log error
            showAlert('Error fetching tournaments.', 'error');  // Error alert
        }
    }

    // Handle match form submission
    if (matchForm) {
        matchForm.addEventListener('submit', async (e) => {
            e.preventDefault();  // Prevent default form submission
            const teamAId = document.getElementById('teamAId').value;
            const teamBId = document.getElementById('teamBId').value;
            const tournamentId = document.getElementById('tournamentId').value;

            try {
                // Send POST request to create a new match
                const response = await fetch('http://localhost:5000/api/match', { 
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'  // Set content type to JSON
                    },
                    body: JSON.stringify({
                        teamAId,
                        teamBId,
                        tournamentId,
                    })  // Convert data to JSON
                });

                if (response.ok) {
                    alert('Match created successfully!');  // Success alert
                    matchForm.reset();  // Reset form fields
                } else {
                    alert('Error creating the match.');  // Error alert
                }
            } catch (error) {
                console.error('Error:', error);  // Log error
                alert('Error creating the match.');  // Error alert
            }
        });
    }

    // Fetch and display all matches
    async function getAllMatches() {
        try {
            const response = await fetch('http://localhost:5000/api/match');
            if (response.ok) {
                const data = await response.json();  // Parse response JSON
                const matchItems = document.getElementById('matchItems');  // Get element for match list
                matchItems.innerHTML = '';  // Clear existing content
                data.forEach(match => {
                    const li = document.createElement('li');  // Create new list item
                    li.textContent = `ID: ${match.id}, Team A: ${match.teamA ? match.teamA.name : 'Not available'}, Team B: ${match.teamB ? match.teamB.name : 'Not available'}, Tournament: ${match.tournament ? match.tournament.name : 'Not available'}, Winner: ${match.winner ? match.winner.name : 'Not available'}`;  // Set text content
                    matchItems.appendChild(li);  // Add list item to the list
                });
            } else {
                showAlert('Error fetching matches.', 'error');  // Error alert
            }
        } catch (error) {
            console.error('Error:', error);  // Log error
            showAlert('Error fetching matches.', 'error');  // Error alert
        }
    }

    // Initialize and display lists of teams, tournaments, and matches
    getAllTeams();  // Fetch and display teams
    getAllTournaments();  // Fetch and display tournaments
    getAllMatches();  // Fetch and display matches
});
