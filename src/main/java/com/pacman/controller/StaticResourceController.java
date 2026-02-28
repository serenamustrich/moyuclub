package com.pacman.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;

@Controller
public class StaticResourceController {
    
    @RequestMapping(value = {"/leaderboard", "/login", "/register", "/game", "/tetris", "/tetris-leaderboard", "/tank-game", "/mbti-test", "/bomberman-game"}, method = RequestMethod.GET)
    public String redirectToIndex() {
        return "forward:/index.html";
    }
}
