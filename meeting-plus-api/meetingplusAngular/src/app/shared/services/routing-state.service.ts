import { Injectable } from '@angular/core';
import { Router, NavigationEnd, RoutesRecognized } from '@angular/router';
import { filter, pairwise } from 'rxjs/operators';

@Injectable()
export class RoutingStateService {
    private history = [];
    returnUrl: string;
    constructor(
        private router: Router
    ) { }

    public loadRouting(): void {
        this.router.events
            .pipe(filter((evt: any) => evt instanceof RoutesRecognized), pairwise())
            .subscribe((events: RoutesRecognized[]) => {
                this.returnUrl = events[0].urlAfterRedirects;
            });
    }

    public getHistory(): string[] {
        return this.history;
    }

    public getPreviousUrl(): string {
        return this.returnUrl;
    }
}