# Save The Set - API

**[sts.sionsmallman.com](https://sts.sionsmallman.com/)**

| Main Page | Setlist customization page |
| --- | --- | 
| ![alt text](https://github.com/SionSmallman/SionSmallmanDotCom/blob/main/public/project-images/sts/1.png?raw=true) | ![alt text](https://github.com/SionSmallman/SionSmallmanDotCom/blob/main/public/project-images/sts/2.png?raw=true) |


The backend API of Save The Set, a web application for quickly and easily converting setlists from setlist.fm into Spotify playlists in the users account, with customization options.

## Features
- Quickly convert setlists from setlist.fm to Spotify playlists
- Customize the titles, description, privacy settings of the playlist.
- Edit the setlists to include what songs you'd like to include/exclude.
- Authentication through Spotify using OAuth2.

## Stack

The API is built in **.NET Core** and **C#**. **Nunit** and **Selenium** are used as part of the testing framework for unit and end-to-end tests. Hosted on Azure Web Services.

## Motivation

This was initially a python script I used personally to prepare for gigs I attended. However, after discussing with some friends who also had the desire for a quick way to add setlist.fm setlists directly to Spotify, I decided to turn the project into a web application for them and others to use. 
